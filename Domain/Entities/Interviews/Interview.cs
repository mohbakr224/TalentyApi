using Domain.Common;
using Domain.Entities.Candidates;
using Domain.Entities.Companies;
using Domain.Entities.Identity;
using Domain.Entities.Jobs;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Interviews
{
    public class Interview : BaseEntity, ISoftDeletable
    {
        public Guid ApplicationId { get; set; }

        public Guid CandidateId { get; set; }

        public Guid JobId { get; set; }

        public Guid CompanyId { get; set; }

        public Guid InterviewerId { get; set; }

        public string Title { get; set; } = null!;

        public string InterviewType { get; set; } = "HR"; // HR, Technical, Cultural, Managerial

        public DateTime ScheduledAt { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public string? LocationOrLink { get; set; }

        public string Status { get; set; } = "SCHEDULED"; // SCHEDULED, COMPLETED, CANCELLED, RESCHEDULED

        public string? Notes { get; set; }

        public string? Feedback { get; set; }

        public int? Rating { get; set; } // 1 - 5

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Domain.Entities.Applications.Application Application { get; set; } = null!;

        public CandidateProfile Candidate { get; set; } = null!;

        public Job Job { get; set; } = null!;

        public Company Company { get; set; } = null!;

        public User Interviewer { get; set; } = null!;

        public ICollection<InterviewSession> Sessions { get; set; } = new List<InterviewSession>();

        public ICollection<InterviewBooking> Bookings { get; set; } = new List<InterviewBooking>();

        public ICollection<InterviewEvaluation> Evaluations { get; set; } = new List<InterviewEvaluation>();

        public ICollection<InterviewReport> Reports { get; set; } = new List<InterviewReport>();
    }
}

