namespace Application.Interfaces.Services;

public interface IAiProvider
{
    Task<string> GenerateAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);
}
