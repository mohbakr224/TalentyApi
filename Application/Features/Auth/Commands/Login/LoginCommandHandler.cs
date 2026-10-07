using Application.DTOs;
using Application.DTOs.Response;
using Application.Interfaces.AuthenticationToken;
using Application.Interfaces.Persistence;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Common.Messaging;
using Common.Results;
using Domain.Entities.AuthenticationToken;
using Domain.Entities.AuthenticationTokenEntities;
using System;
using System.Collections.Generic;
using System.Text;
// removed unused options dependency
namespace Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IcommandHandler<LoginCommand, AuthResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepo _refreshTokenRepo;
        private readonly IJwtService _jwtService;
        private readonly IAccessTokenRepo _accessTokenRepo;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IPasswordHasherServices _passwordHasherServices;
        // Fallback defaults used when JwtSettings is not provided at this layer
        private const int DefaultAccessTokenExpirationMinutes = 60;
        private const int DefaultRefreshTokenExpirationDays = 30;

        private readonly Application.Interfaces.Services.IPermissionService _permissionService;

        public LoginCommandHandler(IUserRepository userRepository, IRefreshTokenRepo refreshTokenRepo, IJwtService jwtService, IUserRoleRepository userRoleRepository, Application.Interfaces.AuthenticationToken.IAccessTokenRepo accessTokenRepo, IPasswordHasherServices passwordHasherServices, Application.Interfaces.Services.IPermissionService permissionService)
        {
            _userRepository = userRepository;
            _refreshTokenRepo = refreshTokenRepo;
            _jwtService = jwtService;
            _userRoleRepository = userRoleRepository;
            _accessTokenRepo = accessTokenRepo;
            _passwordHasherServices = passwordHasherServices;
            _permissionService = permissionService;
        }
        public async Task<ResultT<AuthResponse>> Handle(
     LoginCommand request,
     CancellationToken cancellationToken)
        {
            var loginRequest = request.request;

            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);

            if (user == null)
            {
                return ResultT<AuthResponse>.Failure("Invalid credentials");
            }

            // Verify password
            var passwordValid = _passwordHasherServices.VerifyPassword(loginRequest.Password, user.PasswordHash);
            if (!passwordValid)
            {
                return ResultT<AuthResponse>.Failure("Invalid credentials");
            }

            if (user.IsDeleted)
            {
                return ResultT<AuthResponse>.Failure("User is deleted");
            }

            if (user.IsLocked)
            {
                return ResultT<AuthResponse>.Failure("User is locked");
            }

            // TODO: Verify password here

            var userRoles = user.UserRoles
                .Select(ur => ur.Role.Name)
                .ToList();

            var userPermissions = await _permissionService.GetPermissionsForUserAsync(user.Id);

            // Generate access token on every successful login
            var accessToken = _jwtService.GenerateAccessToken(
                user.Id,
                user.Email,
                userRoles,
                userPermissions);
           await _accessTokenRepo.AddAccessToken(
                new AccessToken
                {
                    UserId = user.Id,
                    Token = accessToken,
                    ExpireAt = DateTime.UtcNow.AddMinutes(DefaultAccessTokenExpirationMinutes)
                },
                cancellationToken);
            // Generate refresh token
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpireAt = DateTime.UtcNow.AddDays(DefaultRefreshTokenExpirationDays)
            };
            var HashedRefreshToken = _jwtService.HashRefreshToken(refreshToken);
            await _refreshTokenRepo.AddRefreshToken(
                refreshTokenEntity,
                cancellationToken);

            return ResultT<AuthResponse>.Success(
      new AuthResponse
      {
          AccessToken = accessToken,
          RefreshToken = refreshToken,
          AccessTokenExpiresAt =DateTime.UtcNow.AddMinutes(DefaultAccessTokenExpirationMinutes),
          UserId = user.Id,
          Email = user.Email,
          Roles = userRoles,
          Permissions = userPermissions
      });
        }
    }
}
