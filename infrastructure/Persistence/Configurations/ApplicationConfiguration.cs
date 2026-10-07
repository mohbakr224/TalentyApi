using Domain.Entities.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Entities.Applications.Application>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Applications.Application> builder)
        {
            builder.ToTable("Applications");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Candidate)
                .WithMany(c => c.Applications)
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
