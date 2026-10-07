using Domain.Common;
using Domain.Entities.Identity;
using System;

namespace Domain.Entities.Interviews
{
    public class InterviewReport : BaseEntity, ISoftDeletable
    {
        public Guid InterviewId { get; set; }

        public Guid CandidateId { get; set; }

        public Guid GeneratedById { get; set; }

        public string ReportType { get; set; } = "COMPREHENSIVE"; // COMPREHENSIVE, SUMMARY, TECHNICAL, BEHAVIORAL

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!; // JSON or Markdown content

        public string? ExecutiveSummary { get; set; }

        public string? FinalRecommendation { get; set; } // HIRE, NO_HIRE, STRONG_HIRE, STRONG_NO_HIRE, NEED_MORE_INFO

        public int? OverallScore { get; set; } // Aggregated score

        public string Status { get; set; } = "DRAFT"; // DRAFT, FINALIZED, SHARED, ARCHIVED

        public DateTime? FinalizedAt { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Interview Interview { get; set; } = null!;

        public User Candidate { get; set; } = null!;

        public User GeneratedBy { get; set; } = null!;
    }
}