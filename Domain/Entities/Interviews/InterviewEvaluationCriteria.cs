using Domain.Common;
using System;

namespace Domain.Entities.Interviews
{
    public class InterviewEvaluationCriteria : BaseEntity, ISoftDeletable
    {
        public Guid EvaluationId { get; set; }

        public string CriteriaName { get; set; } = null!; // Technical Skills, Communication, Problem Solving, Cultural Fit, etc.

        public string? CriteriaCategory { get; set; } // Technical, Soft Skills, Domain Knowledge

        public int Score { get; set; } // 1-5 or 1-10

        public int MaxScore { get; set; } = 5;

        public string? Notes { get; set; }

        public int Weight { get; set; } = 1; // For weighted scoring

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public InterviewEvaluation Evaluation { get; set; } = null!;
    }
}