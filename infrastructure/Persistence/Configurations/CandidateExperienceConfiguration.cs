using Domain.Entities.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class CandidateExperienceConfiguration : IEntityTypeConfiguration<CandidateExperience>
    {
        public void Configure(EntityTypeBuilder<CandidateExperience> builder)
        {
            builder.ToTable("CandidateExperiences");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.JobTitle).IsRequired().HasMaxLength(200);

            builder.HasOne(x => x.Candidate)
                .WithMany(c => c.Experiences)
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
