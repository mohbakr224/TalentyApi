using System;

namespace Application.DTOs.Response
{
    public class JobResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? Location { get; set; }
        public string? WorkType { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? Currency { get; set; }
        public string? Industry { get; set; }
        public string? ExperienceLevel { get; set; }
        public string Status { get; set; } = "DRAFT";
        public DateTime? PublishedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
