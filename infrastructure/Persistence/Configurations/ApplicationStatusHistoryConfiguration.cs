using Domain.Entities.Applications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class ApplicationStatusHistoryConfiguration : IEntityTypeConfiguration<ApplicationStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
        {
            builder.ToTable("ApplicationStatusHistories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ToStatus).IsRequired().HasMaxLength(100);

            builder.HasOne(x => x.Application)
                .WithMany(a => a.StatusHistory)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ChangedByUser)
                .WithMany()
                .HasForeignKey(x => x.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
