using Domain.Entities.Resumes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
    {
        public void Configure(EntityTypeBuilder<Resume> builder)
        {
            builder.ToTable("Resumes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(250);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(100);

            builder.HasMany(x => x.versions)
                .WithOne(v => v.Resume)
                .HasForeignKey(v => v.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
