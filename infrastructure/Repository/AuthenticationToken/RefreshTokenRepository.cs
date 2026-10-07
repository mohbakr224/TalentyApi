using Application.Interfaces.AuthenticationToken;
using Domain.Entities.AuthenticationTokenEntities;
using infrastructure.Persistence;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepo
    {
        private readonly AppDBcontext _context;

        public RefreshTokenRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddRefreshToken(
            RefreshToken token,
            CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(
                token,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRefreshToken(
            RefreshToken token,
            CancellationToken cancellationToken)
        {
            _context.RefreshTokens.Update(token);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<RefreshToken>> GetRefreshTokensByUserId(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _context.RefreshTokens
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
