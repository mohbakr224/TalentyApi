using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Interviews
{
    public class InterviewQuestion : BaseEntity, ISoftDeletable
    {
        public Guid SessionId { get; set; }

        public string QuestionText { get; set; } = null!;

        public string QuestionType { get; set; } = "OpenEnded"; // OpenEnded, MultipleChoice, Coding, TrueFalse, Scale

        public int Order { get; set; }

        public int? MaxScore { get; set; }

        public string? ExpectedAnswer { get; set; }

        public string? Category { get; set; } // Technical, Behavioral, Cultural, DomainKnowledge

        public string? Difficulty { get; set; } // Easy, Medium, Hard

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public InterviewSession Session { get; set; } = null!;

        public ICollection<InterviewAnswer> Answers { get; set; } = new List<InterviewAnswer>();
    }
}