using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateInterviewerAvailabilityRequest
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? TimeZone { get; set; } = "UTC";

        public int SlotDurationMinutes { get; set; } = 30;

        public int MaxBookingsPerSlot { get; set; } = 1;

        public string? InterviewTypes { get; set; } // Comma-separated: HR,Technical,Cultural

        public Guid? JobId { get; set; }

        public Guid? CompanyId { get; set; }

        public string? Notes { get; set; }

        public bool IsRecurring { get; set; } = false;

        public string? RecurrencePattern { get; set; } // Daily, Weekly, Monthly

        public DateTime? RecurrenceEndDate { get; set; }
    }
}