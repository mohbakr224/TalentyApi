using Application.Interfaces.Persistence;
using Domain.Entities.AuthenticationTokenEntities;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.AuthenticationToken
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly AppDBcontext _context;

        public PasswordResetTokenRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token)
        {
            await _context.passwordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        {
            return await _context.passwordResetTokens
                .FirstOrDefaultAsync(x => x.TokenHash == token && x.ExpiresAt > DateTime.UtcNow && !x.UsedAt.HasValue);
        }

        public Task UpdateAsync(PasswordResetToken token)
        {
            _context.passwordResetTokens.Update(token);
            return Task.CompletedTask;
        }
    }
}
