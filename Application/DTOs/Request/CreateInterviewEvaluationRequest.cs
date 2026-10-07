using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateInterviewEvaluationRequest
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public Guid InterviewId { get; set; }

        [Required]
        public Guid CandidateId { get; set; }

        [Required]
        [Range(1, 100)]
        public int OverallScore { get; set; }

        public string? OverallRating { get; set; } // Strong Hire, Hire, No Hire, Strong No Hire

        public string? Summary { get; set; }

        public string? Strengths { get; set; }

        public string? Weaknesses { get; set; }

        public string? Recommendation { get; set; } // Hire, No Hire, Strong Hire, Strong No Hire, Need More Info

        public List<CreateEvaluationCriteriaRequest> CriteriaScores { get; set; } = new();
    }

    public class CreateEvaluationCriteriaRequest
    {
        [Required]
        public string CriteriaName { get; set; } = string.Empty;

        public string? CriteriaCategory { get; set; }

        [Required]
        [Range(1, 10)]
        public int Score { get; set; }

        public int MaxScore { get; set; } = 5;

        public string? Notes { get; set; }

        public int Weight { get; set; } = 1;
    }
}