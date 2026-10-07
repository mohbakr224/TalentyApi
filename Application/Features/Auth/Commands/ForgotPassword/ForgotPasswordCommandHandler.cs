using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Common.Messaging;
using Common.Results;
using Domain.Entities.AuthenticationTokenEntities;
using Domain.Entities.Identity;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IcommandHandler<ForgotPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _emailService = emailService;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                return Result.Success(); // Do not reveal existence

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var tokenEntity = new PasswordResetToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
            };

            await _passwordResetTokenRepository.AddAsync(tokenEntity);

            var resetLink = $"https://localhost:7000/api/v1/auth/reset-password?token={token}";

            await _emailService.SendPasswordResetAsync(user.Email, resetLink);

            return Result.Success();
        }
    }
}
