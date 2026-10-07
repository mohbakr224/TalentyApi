using Application.Interfaces.Persistence;
using Domain.Entities.AuthenticationTokenEntities;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Repository.Identity
{
    public class EmailVerificationRepository
        : IEmailVerificationTokenRepository
    {
        private readonly AppDBcontext _appDBContext;

        public EmailVerificationRepository(
            AppDBcontext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        public async Task AddAsync(
            EmailVerificationTokens token)
        {
            await _appDBContext.EmailVerificationTokens
                .AddAsync(token);

            await _appDBContext.SaveChangesAsync();
        }

        public async Task<EmailVerificationTokens?> GetByTokenAsync(
            string token)
        {
            return await _appDBContext.EmailVerificationTokens
                .FirstOrDefaultAsync(x =>
                    x.Token == token &&
                    !x.IsExpired &&
                    !x.IsUsed);
        }

        public Task UpdateAsync(
            EmailVerificationTokens token)
        {
            _appDBContext.EmailVerificationTokens.Update(token);

            return Task.CompletedTask;
        }
    }
}