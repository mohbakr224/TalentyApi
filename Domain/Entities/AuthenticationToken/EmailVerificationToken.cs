using Domain.Entities.Identity;

namespace Domain.Entities.AuthenticationTokenEntities
{
    public class EmailVerificationTokens
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Token { get; set; } = string.Empty;

        public DateTime ExpireAt { get; set; }

        public DateTime? UsedAt { get; set; }

        public User User { get; set; } = null!;

        public bool IsExpired =>
            DateTime.UtcNow >= ExpireAt;

        public bool IsUsed =>
            UsedAt.HasValue;
    }
}