using System;

namespace Application.DTOs.Response
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? SenderId { get; set; }
        public string? SenderName { get; set; }
        public Guid? CompanyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Guid? RelatedEntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

