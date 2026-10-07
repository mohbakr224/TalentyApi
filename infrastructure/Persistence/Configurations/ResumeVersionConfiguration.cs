using Domain.Entities.Resumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class ResumeVersionConfiguration : IEntityTypeConfiguration<ResumeVersion>
    {
        public void Configure(EntityTypeBuilder<ResumeVersion> builder)
        {
            builder.ToTable("ResumeVersions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.VersionNumber).IsRequired();

            builder.HasOne(x => x.Resume)
                .WithMany(r => r.versions)
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
