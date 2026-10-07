using System;

namespace Application.DTOs.Response
{
    public class InterviewBookingResponse
    {
        public Guid Id { get; set; }
        public Guid AvailabilityId { get; set; }
        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string CandidateEmail { get; set; } = string.Empty;
        public Guid? JobId { get; set; }
        public string? JobTitle { get; set; }
        public Guid? ApplicationId { get; set; }
        public DateTime BookedAt { get; set; }
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
        public Guid? CancelledById { get; set; }
        public string? CancelledByName { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? LocationOrLink { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}