using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewEvaluationConfiguration : IEntityTypeConfiguration<InterviewEvaluation>
    {
        public void Configure(EntityTypeBuilder<InterviewEvaluation> builder)
        {
            builder.ToTable("InterviewEvaluations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OverallRating).HasMaxLength(50);
            builder.Property(x => x.Summary).HasMaxLength(4000);
            builder.Property(x => x.Strengths).HasMaxLength(4000);
            builder.Property(x => x.Weaknesses).HasMaxLength(4000);
            builder.Property(x => x.Recommendation).HasMaxLength(50);

            builder.HasOne(x => x.Session)
                .WithMany(s => s.Evaluations)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Interview)
                .WithMany(i => i.Evaluations)
                .HasForeignKey(x => x.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Evaluator)
                .WithMany()
                .HasForeignKey(x => x.EvaluatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.SessionId);
            builder.HasIndex(x => x.InterviewId);
            builder.HasIndex(x => x.EvaluatorId);
            builder.HasIndex(x => x.CandidateId);
            builder.HasIndex(x => x.IsSubmitted);
        }
    }
}