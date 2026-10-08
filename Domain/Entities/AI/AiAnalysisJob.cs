using Domain.Common;

namespace Domain.Entities.AI;

public sealed class AiAnalysisJob : BaseEntity
{
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string InputJson { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? Result { get; set; }
    public string? Error { get; set; }
    public DateTime? CompletedAt { get; set; }
}
