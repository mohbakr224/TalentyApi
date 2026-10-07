using System;

namespace Application.DTOs.Request
{
    public class UpdateInterviewRequest
    {
        public string? Title { get; set; }
        public string? InterviewType { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string? LocationOrLink { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }
}

