using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateInterviewSessionRequest
    {
        [Required]
        public Guid InterviewId { get; set; }

        [Required]
        public int SessionNumber { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string SessionType { get; set; } = string.Empty; // Technical, HR, Cultural, Managerial, Coding, SystemDesign

        [Required]
        public DateTime ScheduledAt { get; set; }

        public int DurationMinutes { get; set; } = 60;

        public string? LocationOrLink { get; set; }

        public string? Notes { get; set; }

        public Guid? InterviewerId { get; set; }
    }
}