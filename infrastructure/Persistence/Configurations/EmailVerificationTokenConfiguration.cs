using Domain.Entities.AuthenticationTokenEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class EmailVerificationTokenConfiguration
        : IEntityTypeConfiguration<EmailVerificationTokens>
    {
        public void Configure(
            EntityTypeBuilder<EmailVerificationTokens> builder)
        {
            builder.ToTable("EmailVerificationTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired();

            builder.HasIndex(x => x.Token)
                .IsUnique();

            builder.Property(x => x.ExpireAt)
                .IsRequired();

            builder.Property(x => x.UsedAt);

            builder.HasOne(x => x.User)
                .WithMany(u => u.emailVerificationTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(x => x.IsExpired);
            builder.Ignore(x => x.IsUsed);
        }
    }
}