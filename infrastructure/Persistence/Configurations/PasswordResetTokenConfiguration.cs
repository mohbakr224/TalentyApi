using Domain.Entities.AuthenticationTokenEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace infrastructure.Persistence.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UsedAt);
            builder.Property(x => x.ExpiresAt);
            builder.Property(x => x.TokenHash);
            builder.HasOne(x => x.User).WithMany(U=>U.passwordResetTokens);
        }
    }
}
