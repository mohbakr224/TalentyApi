using Domain.Entities.Candidates;
using Domain.Entities.Skills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class CandidateSkillConfiguration : IEntityTypeConfiguration<CandidateSkill>
    {
        public void Configure(EntityTypeBuilder<CandidateSkill> builder)
        {
            builder.ToTable("CandidateSkills");
            builder.HasKey(x => new { x.CandidateId, x.SkillId });

            builder.Property(x => x.ProficiencyLevel).HasMaxLength(100);

            builder.HasOne(x => x.Candidate)
                .WithMany(c => c.Skills)
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Skill)
                .WithMany(s => s.CandidateSkills)
                .HasForeignKey(x => x.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
