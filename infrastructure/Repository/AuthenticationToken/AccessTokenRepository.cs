using Application.Interfaces.AuthenticationToken;
using Domain.Entities.AuthenticationToken;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.AuthenticationToken
{
    public class AccessTokenRepository : IAccessTokenRepo
    {
        private readonly AppDBcontext _context;

        public AccessTokenRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddAccessToken(AccessToken token, CancellationToken cancellationToken)
        {
            await _context.AccessTokens.AddAsync(token, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAccessToken(AccessToken token, CancellationToken cancellationToken)
        {
            _context.AccessTokens.Update(token);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<AccessToken>> GetAccessTokensByUserId(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.AccessTokens.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
        }
    }
}
