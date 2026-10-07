using Domain.Common;
using Domain.Entities.Identity;
using System;

namespace Domain.Entities.Interviews
{
    public class InterviewAnswer : BaseEntity, ISoftDeletable
    {
        public Guid QuestionId { get; set; }

        public Guid CandidateId { get; set; }

        public string AnswerText { get; set; } = null!;

        public int? Score { get; set; }

        public string? EvaluatorNotes { get; set; }

        public Guid? EvaluatedById { get; set; }

        public DateTime? EvaluatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public InterviewQuestion Question { get; set; } = null!;

        public User Candidate { get; set; } = null!;

        public User? EvaluatedBy { get; set; }
    }
}