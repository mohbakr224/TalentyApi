using Domain.Common;
using Domain.Entities.Identity;
using Domain.Entities.Jobs;
using System;

namespace Domain.Entities.Interviews
{
    public class InterviewBooking : BaseEntity, ISoftDeletable
    {
        public Guid AvailabilityId { get; set; }

        public Guid CandidateId { get; set; }

        public Guid? JobId { get; set; }

        public Guid? ApplicationId { get; set; }

        public DateTime BookedAt { get; set; } = DateTime.UtcNow;

        public DateTime ScheduledAt { get; set; }

        public int DurationMinutes { get; set; }

        public string Status { get; set; } = "BOOKED"; // BOOKED, CONFIRMED, CANCELLED_BY_CANDIDATE, CANCELLED_BY_INTERVIEWER, COMPLETED, NO_SHOW

        public string? CancellationReason { get; set; }

        public Guid? CancelledById { get; set; }

        public DateTime? CancelledAt { get; set; }

        public string? LocationOrLink { get; set; }

        public string? Notes { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public InterviewerAvailability Availability { get; set; } = null!;

        public User Candidate { get; set; } = null!;

        public Job? Job { get; set; }

        public Domain.Entities.Applications.Application? Application { get; set; }

        public User? CancelledBy { get; set; }

        public Interview? CreatedInterview { get; set; }
    }
}