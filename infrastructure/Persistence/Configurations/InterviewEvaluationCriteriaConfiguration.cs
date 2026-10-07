using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewEvaluationCriteriaConfiguration : IEntityTypeConfiguration<InterviewEvaluationCriteria>
    {
        public void Configure(EntityTypeBuilder<InterviewEvaluationCriteria> builder)
        {
            builder.ToTable("InterviewEvaluationCriteria");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CriteriaName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CriteriaCategory).HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasOne(x => x.Evaluation)
                .WithMany(e => e.CriteriaScores)
                .HasForeignKey(x => x.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.EvaluationId);
        }
    }
}