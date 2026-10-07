using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.ToTable("Interviews");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
            builder.Property(x => x.InterviewType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LocationOrLink).HasMaxLength(500);

            builder.HasOne(x => x.Application)
                .WithMany()
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Interviewer)
                .WithMany()
                .HasForeignKey(x => x.InterviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

