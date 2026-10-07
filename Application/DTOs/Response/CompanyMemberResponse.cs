using System;

namespace Application.DTOs.Response
{
    public class CompanyMemberResponse
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

