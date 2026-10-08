using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace infrastructure.Services.AI;

/// <summary>Works with OpenAI and OpenAI-compatible chat-completions endpoints.</summary>
public sealed class OpenAiCompatibleProvider : IAiProvider
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;

    public OpenAiCompatibleProvider(HttpClient http, IConfiguration configuration) { _http = http; _configuration = configuration; }

    public async Task<string> GenerateAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        var baseUrl = _configuration["AI:BaseUrl"];
        var model = _configuration["AI:Model"];
        var apiKey = _configuration["AI:ApiKey"];
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(model))
            throw new InvalidOperationException("AI is not configured. Set AI:BaseUrl, AI:Model, and AI:ApiKey (when required) in configuration.");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl.TrimEnd('/')}/chat/completions");
        if (!string.IsNullOrWhiteSpace(apiKey)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(new { model, temperature = 0.2, messages = new[] { new { role = "system", content = systemPrompt }, new { role = "user", content = userPrompt } } }), Encoding.UTF8, "application/json");
        using var response = await _http.SendAsync(request, cancellationToken);
        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException($"AI provider returned {(int)response.StatusCode}: {payload[..Math.Min(payload.Length, 500)]}");
        using var document = JsonDocument.Parse(payload);
        return document.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
    }
}
