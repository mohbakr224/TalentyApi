using Domain.Entities.AI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace infrastructure.Persistence.Configurations;

public sealed class AiAnalysisJobConfiguration : IEntityTypeConfiguration<AiAnalysisJob>
{
    public void Configure(EntityTypeBuilder<AiAnalysisJob> builder)
    {
        builder.ToTable("AiAnalysisJobs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(60).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
        builder.Property(x => x.InputJson).IsRequired();
        builder.HasIndex(x => new { x.UserId, x.CreatedDate });
    }
}
