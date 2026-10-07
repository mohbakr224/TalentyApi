using Application.Interfaces.AuthenticationToken;
using Common.Messaging;
using Common.Results;
using Domain.Entities.AuthenticationTokenEntities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IcommandHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepo _refreshTokenRepo;

        public LogoutCommandHandler(IRefreshTokenRepo refreshTokenRepo)
        {
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var tokens = await _refreshTokenRepo.GetRefreshTokensByUserId(request.UserId, cancellationToken);

            if (tokens == null || !tokens.Any())
            {
                return Result.Success();
            }

            foreach (var t in tokens)
            {
                if (t.RevokeAt == null)
                {
                    t.RevokeAt = DateTime.UtcNow;
                    await _refreshTokenRepo.UpdateRefreshToken(t, cancellationToken);
                }
            }

            return Result.Success();
        }
    }
}
