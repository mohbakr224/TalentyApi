using Domain.Entities.Candidates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class CandidateEducationConfiguration : IEntityTypeConfiguration<CandidateEducation>
    {
        public void Configure(EntityTypeBuilder<CandidateEducation> builder)
        {
            builder.ToTable("CandidateEducations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Institution).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Degree).HasMaxLength(200);

            builder.HasOne(x => x.Candidate)
                .WithMany(c => c.Education)
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
