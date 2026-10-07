using Domain.Entities.AuthenticationTokenEntities;

namespace Application.Interfaces.Persistence
{
    public interface IEmailVerificationTokenRepository
    {
        Task AddAsync(EmailVerificationTokens token);

        Task<EmailVerificationTokens?> GetByTokenAsync(
            string token);

        Task UpdateAsync(
            EmailVerificationTokens token);
    }
}