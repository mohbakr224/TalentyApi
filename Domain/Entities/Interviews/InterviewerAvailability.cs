using Domain.Common;
using Domain.Entities.Companies;
using Domain.Entities.Identity;
using Domain.Entities.Jobs;
using System;

namespace Domain.Entities.Interviews
{
    public class InterviewerAvailability : BaseEntity, ISoftDeletable
    {
        public Guid InterviewerId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string? TimeZone { get; set; } = "UTC";

        public int SlotDurationMinutes { get; set; } = 30;

        public int MaxBookingsPerSlot { get; set; } = 1;

        public string? InterviewTypes { get; set; } // Comma-separated: HR,Technical,Cultural

        public Guid? JobId { get; set; } // Optional: specific job this availability is for

        public Guid? CompanyId { get; set; }

        public string Status { get; set; } = "AVAILABLE"; // AVAILABLE, BLOCKED, FULLY_BOOKED

        public string? Notes { get; set; }

        public bool IsRecurring { get; set; } = false;

        public string? RecurrencePattern { get; set; } // Daily, Weekly, Monthly

        public DateTime? RecurrenceEndDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public User Interviewer { get; set; } = null!;

        public Job? Job { get; set; }

        public Company? Company { get; set; }

        public ICollection<InterviewBooking> Bookings { get; set; } = new List<InterviewBooking>();
    }
}