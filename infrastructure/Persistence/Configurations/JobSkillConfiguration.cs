using Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class JobSkillConfiguration : IEntityTypeConfiguration<JobSkill>
    {
        public void Configure(EntityTypeBuilder<JobSkill> builder)
        {
            builder.ToTable("JobSkills");
            builder.HasKey(x => new { x.JobId, x.SkillId });

            builder.Property(x => x.ProficiencyLevel).HasMaxLength(100);

            builder.HasOne(x => x.Job)
                .WithMany(j => j.Skills)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Skill)
                .WithMany(s => s.JobSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
