namespace Application.Interfaces.Services;

public interface IAiAnalysisService
{
    Task<string> AnalyzeAsync(string type, string inputJson, CancellationToken cancellationToken = default);
}
