using System;

namespace Application.DTOs.Request
{
    public class UpdateInterviewerAvailabilityRequest
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? TimeZone { get; set; }
        public int? SlotDurationMinutes { get; set; }
        public int? MaxBookingsPerSlot { get; set; }
        public string? InterviewTypes { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public bool? IsRecurring { get; set; }
        public string? RecurrencePattern { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
    }
}