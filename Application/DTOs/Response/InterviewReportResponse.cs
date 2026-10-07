using System;

namespace Application.DTOs.Response
{
    public class InterviewReportResponse
    {
        public Guid Id { get; set; }
        public Guid InterviewId { get; set; }
        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public Guid GeneratedById { get; set; }
        public string GeneratedByName { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ExecutiveSummary { get; set; }
        public string? FinalRecommendation { get; set; }
        public int? OverallScore { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? FinalizedAt { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}