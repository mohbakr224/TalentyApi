using Domain.Common;
using Domain.Entities.Identity;
using System;

namespace Domain.Entities.Interviews
{
    public class InterviewEvaluation : BaseEntity, ISoftDeletable
    {
        public Guid SessionId { get; set; }

        public Guid InterviewId { get; set; }

        public Guid EvaluatorId { get; set; }

        public Guid CandidateId { get; set; }

        public int OverallScore { get; set; } // 1-100 or 1-10

        public string? OverallRating { get; set; } // Strong Hire, Hire, No Hire, Strong No Hire

        public string? Summary { get; set; }

        public string? Strengths { get; set; }

        public string? Weaknesses { get; set; }

        public string? Recommendation { get; set; } // Hire, No Hire, Strong Hire, Strong No Hire, Need More Info

        public bool IsSubmitted { get; set; } = false;

        public DateTime? SubmittedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public InterviewSession Session { get; set; } = null!;

        public Interview Interview { get; set; } = null!;

        public User Evaluator { get; set; } = null!;

        public User Candidate { get; set; } = null!;

        public ICollection<InterviewEvaluationCriteria> CriteriaScores { get; set; } = new List<InterviewEvaluationCriteria>();
    }
}