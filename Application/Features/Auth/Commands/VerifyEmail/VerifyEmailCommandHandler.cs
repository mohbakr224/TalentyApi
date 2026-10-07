using Application.Interfaces.Persistence;
using Common.Messaging;
using Common.Results;

namespace Application.Features.Auth.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler
        : IcommandHandler<VerifyEmailCommand>
    {
        private readonly IEmailVerificationTokenRepository
            _emailVerificationRepository;

        private readonly IUserRepository
            _userRepository;

        public VerifyEmailCommandHandler(
            IEmailVerificationTokenRepository
                emailVerificationRepository,
            IUserRepository userRepository)
        {
            _emailVerificationRepository =
                emailVerificationRepository;

            _userRepository = userRepository;
        }

        public async Task<Result> Handle(
            VerifyEmailCommand command,
            CancellationToken cancellationToken)
        {
            var verificationToken =
                await _emailVerificationRepository
                    .GetByTokenAsync(command.Token);

            if (verificationToken == null)
            {
                return Result.Failure(
                    "Invalid or expired verification token.");
            }

            var user =
                await _userRepository
                    .GetUserByIdAsync(verificationToken.UserId);

            if (user == null)
            {
                return Result.Failure(
                    "User was not found.");
            }

            user.EmailVerified = true;

            verificationToken.UsedAt =
                DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            await _emailVerificationRepository
                .UpdateAsync(verificationToken);
            return Result.Success();
        }
    }
}