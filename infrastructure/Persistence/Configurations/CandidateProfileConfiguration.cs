using Domain.Entities.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class CandidateProfileConfiguration : IEntityTypeConfiguration<CandidateProfile>
    {
        public void Configure(EntityTypeBuilder<CandidateProfile> builder)
        {
            builder.ToTable("CandidateProfiles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Headline).HasMaxLength(250);
            builder.Property(x => x.Location).HasMaxLength(200);

            builder.Property(x => x.CreatedDate).IsRequired();

            builder.HasOne(x => x.User)
                .WithOne(u => u.CandidateProfile)
                .HasForeignKey<CandidateProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
