using Domain.Common;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Interviews
{
    public class InterviewSession : BaseEntity, ISoftDeletable
    {
        public Guid InterviewId { get; set; }

        public int SessionNumber { get; set; }

        public string Title { get; set; } = null!;

        public string SessionType { get; set; } = null!; // Technical, HR, Cultural, Managerial, Coding, SystemDesign

        public DateTime ScheduledAt { get; set; }

        public int DurationMinutes { get; set; } = 60;

        public string? LocationOrLink { get; set; }

        public string Status { get; set; } = "SCHEDULED"; // SCHEDULED, IN_PROGRESS, COMPLETED, CANCELLED, RESCHEDULED

        public string? Notes { get; set; }

        public string? Feedback { get; set; }

        public int? Rating { get; set; } // 1-5 overall session rating

        public Guid? InterviewerId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Interview Interview { get; set; } = null!;

        public User? Interviewer { get; set; }

        public ICollection<InterviewQuestion> Questions { get; set; } = new List<InterviewQuestion>();

        public ICollection<InterviewEvaluation> Evaluations { get; set; } = new List<InterviewEvaluation>();
    }
}