using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Common.Messaging;
using Common.Results;
using Domain.Entities.AuthenticationTokenEntities;
using Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IcommandHandler<ForgotPasswordCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IEmailService _emailService;
        private readonly string _authBaseUrl;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _emailService = emailService;
            _authBaseUrl = configuration["Auth:BaseUrl"]?.TrimEnd('/')
                ?? throw new InvalidOperationException("Auth:BaseUrl must be configured.");
        }

        public async Task<ResultT<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return ResultT<string>.Success(string.Empty); // Do not reveal existence

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var tokenEntity = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
            };

            await _passwordResetTokenRepository.AddAsync(tokenEntity);

            var resetLink = $"{_authBaseUrl}/api/v1/auth/reset-password?token={Uri.EscapeDataString(token)}";

            // Don't fail the request if email delivery fails
            try
            {
                await _emailService.SendPasswordResetAsync(user.Email, resetLink);
            }
            catch (Exception)
            {
                // Email delivery failed — still return success to not reveal user existence
                // and because the reset token is saved in DB for retry.
            }

            return ResultT<string>.Success(token);
        }
    }
}

