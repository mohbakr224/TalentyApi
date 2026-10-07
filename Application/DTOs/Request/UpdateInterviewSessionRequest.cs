using System;

namespace Application.DTOs.Request
{
    public class UpdateInterviewSessionRequest
    {
        public string? Title { get; set; }
        public string? SessionType { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string? LocationOrLink { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string? Feedback { get; set; }
        public int? Rating { get; set; }
        public Guid? InterviewerId { get; set; }
    }
}