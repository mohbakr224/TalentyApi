using System;

namespace Application.DTOs.Response
{
    public class InterviewResponse
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid CandidateId { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public Guid JobId { get; set; }
        public string? JobTitle { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public Guid InterviewerId { get; set; }
        public string? InterviewerName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string InterviewType { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public string? LocationOrLink { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Feedback { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

