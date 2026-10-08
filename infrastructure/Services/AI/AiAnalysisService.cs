using Application.Interfaces.Services;

namespace infrastructure.Services.AI;

public sealed class AiAnalysisService : IAiAnalysisService
{
    private readonly IAiProvider _provider;
    public AiAnalysisService(IAiProvider provider) => _provider = provider;

    public Task<string> AnalyzeAsync(string type, string inputJson, CancellationToken cancellationToken = default)
    {
        var task = type.ToLowerInvariant() switch
        {
            "cv-analysis" => "Analyze this CV. Return a concise structured report with summary, skills, experience, strengths, gaps, and actionable improvements.",
            "portfolio-analysis" => "Analyze this portfolio for relevance, quality, evidence of impact, strengths, gaps, and improvements.",
            "job-description-analysis" => "Analyze this job description. Extract role, responsibilities, required and preferred skills, seniority, and ambiguities.",
            "ai-interview" => "Create a role-specific interview plan with questions, what each question evaluates, and a suggested difficulty. Use provided candidate/job context.",
            "interview-evaluation" => "Evaluate the supplied interview answers against the role. Give evidence-based per-skill scores, strengths, concerns, and a recommendation. Do not infer protected characteristics.",
            "career-analysis" => "Analyze the candidate profile and suggest career paths, skill gaps, and a practical prioritized learning plan.",
            "matching" => "Match the candidate profile to the job. Return an overall 0-100 fit score, matching skills, gaps, evidence, and recommendations. Do not use protected characteristics.",
            _ => throw new InvalidOperationException("Unsupported AI analysis type.")
        };
        return _provider.GenerateAsync("You are an assistant for a recruitment platform. Be factual, identify uncertainty, and return JSON when practical. Do not make decisions based on protected characteristics.", $"Task: {task}\n\nInput JSON:\n{inputJson}", cancellationToken);
    }
}
