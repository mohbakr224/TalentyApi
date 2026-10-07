using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateCompanyRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Industry { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }
    }
}
