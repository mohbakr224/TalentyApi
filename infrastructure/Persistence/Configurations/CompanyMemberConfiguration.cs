using Domain.Entities.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
    {
        public void Configure(EntityTypeBuilder<CompanyMember> builder)
        {
            builder.ToTable("CompanyMembers");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Role).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Company)
                .WithMany(c => c.Members)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(u => u.CompanyMemberships)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
