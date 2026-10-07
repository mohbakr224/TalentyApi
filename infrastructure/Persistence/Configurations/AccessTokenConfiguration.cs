using Domain.Entities.AuthenticationToken;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace infrastructure.Persistence.Configurations
{
    public class AccessTokenConfiguration : IEntityTypeConfiguration<AccessToken>
    {
        public void Configure(EntityTypeBuilder<AccessToken> builder)
        {
            builder.ToTable("AccessTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired();

            builder.Property(x => x.ExpireAt)
                .IsRequired();

            builder.Property(x => x.RevokeAt);

            builder.HasOne(x => x.User)
                .WithMany(u => u.AccessTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore(x => x.IsExpired);
        }
    }
}
