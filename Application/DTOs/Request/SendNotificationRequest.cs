using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class SendNotificationRequest
    {
        [Required]
        public Guid UserId { get; set; }

        public Guid? CompanyId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = "RECRUITMENT"; // APPLICATION_STATUS, INTERVIEW_INVITATION, RECRUITMENT, JOB_UPDATE, GENERAL

        public Guid? RelatedEntityId { get; set; }
    }
}

