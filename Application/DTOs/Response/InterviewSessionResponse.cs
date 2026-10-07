using System;

namespace Application.DTOs.Response
{
    public class InterviewSessionResponse
    {
        public Guid Id { get; set; }
        public Guid InterviewId { get; set; }
        public int SessionNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SessionType { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public string? LocationOrLink { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Feedback { get; set; }
        public int? Rating { get; set; }
        public Guid? InterviewerId { get; set; }
        public string? InterviewerName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}