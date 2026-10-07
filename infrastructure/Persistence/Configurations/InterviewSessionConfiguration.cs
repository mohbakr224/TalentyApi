using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewSessionConfiguration : IEntityTypeConfiguration<InterviewSession>
    {
        public void Configure(EntityTypeBuilder<InterviewSession> builder)
        {
            builder.ToTable("InterviewSessions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
            builder.Property(x => x.SessionType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LocationOrLink).HasMaxLength(500);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.Feedback).HasMaxLength(4000);

            builder.HasOne(x => x.Interview)
                .WithMany(i => i.Sessions)
                .HasForeignKey(x => x.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Interviewer)
                .WithMany()
                .HasForeignKey(x => x.InterviewerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => x.InterviewId);
            builder.HasIndex(x => x.ScheduledAt);
            builder.HasIndex(x => x.Status);
        }
    }
}