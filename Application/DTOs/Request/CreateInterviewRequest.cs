using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateInterviewRequest
    {
        [Required]
        public Guid ApplicationId { get; set; }

        [Required]
        public string Title { get; set; } = "Recruitment Interview";

        public string InterviewType { get; set; } = "HR"; // HR, Technical, Cultural, Managerial

        [Required]
        public DateTime ScheduledAt { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public string? LocationOrLink { get; set; }

        public string? Notes { get; set; }
    }
}

