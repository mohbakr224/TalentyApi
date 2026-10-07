using System;

namespace Application.DTOs.Response
{
    public class InterviewerAvailabilityResponse
    {
        public Guid Id { get; set; }
        public Guid InterviewerId { get; set; }
        public string InterviewerName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TimeZone { get; set; } = "UTC";
        public int SlotDurationMinutes { get; set; }
        public int MaxBookingsPerSlot { get; set; }
        public int CurrentBookingsCount { get; set; }
        public string? InterviewTypes { get; set; }
        public Guid? JobId { get; set; }
        public string? JobTitle { get; set; }
        public Guid? CompanyId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsRecurring { get; set; }
        public string? RecurrencePattern { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}