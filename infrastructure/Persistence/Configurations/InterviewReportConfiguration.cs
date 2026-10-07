using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewReportConfiguration : IEntityTypeConfiguration<InterviewReport>
    {
        public void Configure(EntityTypeBuilder<InterviewReport> builder)
        {
            builder.ToTable("InterviewReports");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReportType).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Content).IsRequired();
            builder.Property(x => x.ExecutiveSummary).HasMaxLength(4000);
            builder.Property(x => x.FinalRecommendation).HasMaxLength(50);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);

            builder.HasOne(x => x.Interview)
                .WithMany(i => i.Reports)
                .HasForeignKey(x => x.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GeneratedBy)
                .WithMany()
                .HasForeignKey(x => x.GeneratedById)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.InterviewId);
            builder.HasIndex(x => x.CandidateId);
            builder.HasIndex(x => x.Status);
        }
    }
}