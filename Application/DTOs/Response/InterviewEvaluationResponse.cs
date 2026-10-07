using System;

namespace Application.DTOs.Response
{
    public class InterviewEvaluationResponse
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public Guid InterviewId { get; set; }
        public Guid EvaluatorId { get; set; }
        public string EvaluatorName { get; set; } = string.Empty;
        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public int OverallScore { get; set; }
        public string? OverallRating { get; set; }
        public string? Summary { get; set; }
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public string? Recommendation { get; set; }
        public bool IsSubmitted { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<InterviewEvaluationCriteriaResponse> CriteriaScores { get; set; } = new();
    }

    public class InterviewEvaluationCriteriaResponse
    {
        public Guid Id { get; set; }
        public string CriteriaName { get; set; } = string.Empty;
        public string? CriteriaCategory { get; set; }
        public int Score { get; set; }
        public int MaxScore { get; set; }
        public string? Notes { get; set; }
        public int Weight { get; set; }
    }
}