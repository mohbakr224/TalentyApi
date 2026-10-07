using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateJobRequest
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(100)]
        public string? WorkType { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? Currency { get; set; }
        public string? Industry { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? Status { get; set; } = "DRAFT"; // DRAFT or PUBLISHED
        public DateTime? ClosedAt { get; set; }
        public List<Guid>? SkillIds { get; set; }
    }
}
