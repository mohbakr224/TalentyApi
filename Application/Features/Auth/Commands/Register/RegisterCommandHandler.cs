using Application.DTOs.Response;
using Application.Interfaces.Persistence;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Common.Messaging;
using Common.Results;
using Domain.Entities.AuthenticationTokenEntities;
using Domain.Entities.Identity;
using Domain.Enums;
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

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUserRoleRepository userRoleRepository,
            IEmailVerificationTokenRepository emailVerificationRepository,
            IPasswordHasherServices passwordHasher,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _emailVerificationRepository =
                emailVerificationRepository;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
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

            // 2. Get CANDIDATE role
            var candidateRole =
                await _roleRepository.GetByNameAsync(
                    RoleType.CANDIDATE);

            if (candidateRole == null)
            {
                return ResultT<AuthResponse>.Failure(
                    "CANDIDATE role was not found.");
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

            // 4. Assign CANDIDATE role
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = candidateRole.Id
            };

            await _userRoleRepository.AddASync(userRole);

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
                $"https://localhost:7000/api/v1/auth/verify-email" +
                $"?token={verificationToken}";

            // 8. Send verification email
            await _emailService.SendEmailVerificationAsync(
                user.Email,
                verificationLink);
            // 10. Return response
            var response = new AuthResponse
            {
                UserId = user.Id,

                Email = user.Email,

                Roles = new List<string>
                {
                    candidateRole.Name
                }
            };

            return ResultT<AuthResponse>.Success(response);
        }
    }
}