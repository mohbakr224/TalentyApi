using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(3)]
        public string LastName { get; set; } = string.Empty;

        [RegularExpression("^(?i:CANDIDATE|HR)$", ErrorMessage = "Role must be CANDIDATE or HR.")]
        public string Role { get; set; } = nameof(RoleType.CANDIDATE);

        public Guid? CompanyId { get; set; }
    }
}
