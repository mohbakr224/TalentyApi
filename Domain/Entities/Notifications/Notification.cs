using Domain.Common;
using Domain.Entities.Identity;
using System;

namespace Domain.Entities.Notifications
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }

        public Guid? SenderId { get; set; }

        public Guid? CompanyId { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string Type { get; set; } = "RECRUITMENT"; // APPLICATION_STATUS, INTERVIEW_INVITATION, RECRUITMENT, JOB_UPDATE, GENERAL

        public Guid? RelatedEntityId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }

        public User User { get; set; } = null!;

        public User? Sender { get; set; }
    }
}

