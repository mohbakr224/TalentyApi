using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewAnswerConfiguration : IEntityTypeConfiguration<InterviewAnswer>
    {
        public void Configure(EntityTypeBuilder<InterviewAnswer> builder)
        {
            builder.ToTable("InterviewAnswers");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AnswerText).IsRequired().HasMaxLength(4000);
            builder.Property(x => x.EvaluatorNotes).HasMaxLength(2000);

            builder.HasOne(x => x.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Candidate)
                .WithMany()
                .HasForeignKey(x => x.CandidateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.EvaluatedBy)
                .WithMany()
                .HasForeignKey(x => x.EvaluatedById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => x.QuestionId);
            builder.HasIndex(x => x.CandidateId);
        }
    }
}