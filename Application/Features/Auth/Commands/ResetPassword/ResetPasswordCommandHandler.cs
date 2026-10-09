using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Common.Messaging;
using Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IcommandHandler<ResetPasswordCommand>
    {
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherServices _passwordHasher;

        public ResetPasswordCommandHandler(
            IPasswordResetTokenRepository passwordResetTokenRepository,
            IUserRepository userRepository,
            IPasswordHasherServices passwordHasher)
        {
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var tokenEntity = await _passwordResetTokenRepository.GetByTokenAsync(request.Token);

            if (tokenEntity == null)
            {
                return Result.Failure("Invalid or expired token");
            }

            var user = await _userRepository.GetUserByIdAsync(tokenEntity.UserId);

            if (user == null)
                return Result.Failure("User not found");

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);

            await _userRepository.UpdateAsync(user);

            tokenEntity.UsedAt = System.DateTime.UtcNow;
            await _passwordResetTokenRepository.UpdateAsync(tokenEntity);
            await _userRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
