using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewBookingConfiguration : IEntityTypeConfiguration<InterviewBooking>
    {
        public void Configure(EntityTypeBuilder<InterviewBooking> builder)
        {
            builder.ToTable("InterviewBookings");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CancellationReason).HasMaxLength(500);
            builder.Property(x => x.LocationOrLink).HasMaxLength(500);
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasOne(x => x.Availability)
                .WithMany(a => a.Bookings)
                .HasForeignKey(x => x.AvailabilityId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Application)
                .WithMany()
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CancelledBy)
                .WithMany()
                .HasForeignKey(x => x.CancelledById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CreatedInterview)
                .WithMany()
                .HasForeignKey(x => x.Id)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasIndex(x => x.AvailabilityId);
            builder.HasIndex(x => x.CandidateId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.ScheduledAt);
            builder.HasIndex(x => new { x.AvailabilityId, x.CandidateId, x.Status });
        }
    }
}