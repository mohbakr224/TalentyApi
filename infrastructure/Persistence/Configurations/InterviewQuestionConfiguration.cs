using Domain.Entities.Interviews;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations
{
    public class InterviewQuestionConfiguration : IEntityTypeConfiguration<InterviewQuestion>
    {
        public void Configure(EntityTypeBuilder<InterviewQuestion> builder)
        {
            builder.ToTable("InterviewQuestions");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.QuestionText).IsRequired().HasMaxLength(4000);
            builder.Property(x => x.QuestionType).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ExpectedAnswer).HasMaxLength(4000);
            builder.Property(x => x.Category).HasMaxLength(100);
            builder.Property(x => x.Difficulty).HasMaxLength(20);

            builder.HasOne(x => x.Session)
                .WithMany(s => s.Questions)
                .HasForeignKey(x => x.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.SessionId);
            builder.HasIndex(x => x.Order);
        }
    }
}