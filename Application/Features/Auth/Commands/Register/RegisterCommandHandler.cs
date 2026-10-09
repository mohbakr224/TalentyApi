using Application.DTOs.Response;
using Application.DTOs.Request;
using Application.Interfaces.Persistence;
using Application.Interfaces.AuthenticationToken;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Common.Messaging;
using Common.Results;
using Domain.Entities.AuthenticationTokenEntities;
using Domain.Entities.AuthenticationToken;
using Domain.Entities.Identity;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler
        : IcommandHandler<RegisterCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IEmailVerificationTokenRepository
            _emailVerificationRepository;
        private readonly IPasswordHasherServices _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly IAccessTokenRepo _accessTokenRepo;
        private readonly IRefreshTokenRepo _refreshTokenRepo;
        private readonly IPermissionService _permissionService;
        private readonly ICompanyService _companyService;
        private readonly string _authBaseUrl;

        private const int AccessTokenExpirationMinutes = 60;
        private const int RefreshTokenExpirationDays = 30;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IEmailVerificationTokenRepository emailVerificationRepository,
            IPasswordHasherServices passwordHasher,
            IEmailService emailService,
            IJwtService jwtService,
            IAccessTokenRepo accessTokenRepo,
            IRefreshTokenRepo refreshTokenRepo,
            IPermissionService permissionService,
            ICompanyService companyService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _emailVerificationRepository =
                emailVerificationRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _jwtService = jwtService;
            _accessTokenRepo = accessTokenRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _permissionService = permissionService;
            _companyService = companyService;
            _authBaseUrl = configuration["Auth:BaseUrl"]?.TrimEnd('/')
                ?? throw new InvalidOperationException("Auth:BaseUrl must be configured.");
        }

        public async Task<ResultT<AuthResponse>> Handle(
            RegisterCommand command,
            CancellationToken cancellationToken)
        {
            var request = command.Request;

            // 1. Check if email already exists
            var existingUser =
                await _userRepository.GetByEmailAsync(
                    request.Email);

            if (existingUser != null)
            {
                return ResultT<AuthResponse>.Failure(
                    "The email already exists.");
            }

            var requestedRole = request.Role?.Trim();
            if (!string.Equals(requestedRole, nameof(RoleType.CANDIDATE), StringComparison.OrdinalIgnoreCase)
                && !string.Equals(requestedRole, nameof(RoleType.HR), StringComparison.OrdinalIgnoreCase))
            {
                return ResultT<AuthResponse>.Failure("Role must be CANDIDATE or HR.");
            }

            var isHr = string.Equals(requestedRole, nameof(RoleType.HR), StringComparison.OrdinalIgnoreCase);
            if (isHr && !request.CompanyId.HasValue)
            {
                return ResultT<AuthResponse>.Failure("CompanyId is required when registering as HR.");
            }

            if (!isHr && request.CompanyId.HasValue)
            {
                return ResultT<AuthResponse>.Failure("CompanyId can only be provided when registering as HR.");
            }

            if (isHr && await _companyService.GetByIdAsync(request.CompanyId!.Value) == null)
            {
                return ResultT<AuthResponse>.Failure("The specified company was not found.");
            }

            // 2. Get the requested role
            var candidateRole =
                await _roleRepository.GetByNameAsync(
                    isHr ? RoleType.HR : RoleType.CANDIDATE);

            if (candidateRole == null)
            {
                return ResultT<AuthResponse>.Failure(
                    $"{(isHr ? "HR" : "CANDIDATE")} role was not found.");
            }

            // 3. Create user
            var user = new User
            {
                Id = Guid.NewGuid(),

                Email = request.Email,

                PasswordHash =
                    _passwordHasher.HashPassword(
                        request.Password),

                FirstName = request.FirstName,
                LastName = request.LastName,

                CreatedDate = DateTime.UtcNow,

                IsActive = true,
                EmailVerified = false,
                IsLocked = false
            };

            await _userRepository.AddAsync(user);

            // 4. Assign the selected system role
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = candidateRole.Id
            };

            await _userRoleRepository.AddASync(userRole);

            if (isHr)
            {
                await _companyService.AddMemberAsync(
                    request.CompanyId!.Value,
                    new AddCompanyMemberRequest { UserId = user.Id, Role = nameof(RoleType.HR) },
                    user.Id);
            }

            // 5. Generate verification token
            var verificationToken =
                Convert.ToBase64String(
                    RandomNumberGenerator.GetBytes(32));

            // 6. Save verification token in database
            var emailVerificationToken =
                new EmailVerificationTokens
                {
                    Id = Guid.NewGuid(),

                    UserId = user.Id,

                    Token = verificationToken,

                    ExpireAt =
                        DateTime.UtcNow.AddHours(24)
                };

            await _emailVerificationRepository.AddAsync(
                emailVerificationToken);

            // 7. Create verification link
            var verificationLink =
                $"{_authBaseUrl}/api/v1/auth/verify-email" +
                $"?token={Uri.EscapeDataString(verificationToken)}";

            // 8. Send verification email (don't fail registration if email fails)
            try
            {
                await _emailService.SendEmailVerificationAsync(
                    user.Email,
                    verificationLink);
            }
            catch (Exception)
            {
                // Email delivery failed — registration still succeeds.
                // The verification token is saved in DB; user can request resend.
            }

            var roles = new List<string> { candidateRole.Name };
            var permissions = await _permissionService.GetPermissionsForUserAsync(user.Id);
            var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, roles, permissions);
            var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes);

            await _accessTokenRepo.AddAccessToken(
                new AccessToken
                {
                    UserId = user.Id,
                    Token = accessToken,
                    ExpireAt = accessTokenExpiresAt
                },
                cancellationToken);

            var refreshToken = _jwtService.GenerateRefreshToken();
            await _refreshTokenRepo.AddRefreshToken(
                new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpireAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays)
                },
                cancellationToken);

            var response = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles,
                Permissions = permissions,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = accessTokenExpiresAt
            };

            return ResultT<AuthResponse>.Success(response);
        }
    }
}

