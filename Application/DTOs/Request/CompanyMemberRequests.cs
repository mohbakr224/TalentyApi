using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class AddCompanyMemberRequest
    {
        public Guid? UserId { get; set; }

        public string? Email { get; set; }

        [Required]
        public string Role { get; set; } = "HR"; // HR, RECRUITER, COMPANY_ADMIN
    }

    public class UpdateCompanyMemberRequest
    {
        public string? Role { get; set; }

        public bool? IsActive { get; set; }
    }
}

