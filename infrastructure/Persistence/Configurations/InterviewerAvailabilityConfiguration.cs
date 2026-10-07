using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewerAvailabilityConfiguration : IEntityTypeConfiguration<InterviewerAvailability>
    {
        public void Configure(EntityTypeBuilder<InterviewerAvailability> builder)
        {
            builder.ToTable("InterviewerAvailabilities");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TimeZone).HasMaxLength(50);
            builder.Property(x => x.InterviewTypes).HasMaxLength(200);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.RecurrencePattern).HasMaxLength(50);

            builder.HasOne(x => x.Interviewer)
                .WithMany()
                .HasForeignKey(x => x.InterviewerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.InterviewerId);
            builder.HasIndex(x => x.StartTime);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => new { x.InterviewerId, x.StartTime, x.EndTime });
        }
    }
}