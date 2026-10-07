using System;
using System.Collections.Generic;

namespace Application.DTOs.Request
{
    public class UpdateJobRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public string? WorkType { get; set; }
        public string? ExperienceLevel { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? Currency { get; set; }
        public DateTime? ClosedAt { get; set; }
        public List<Guid>? SkillIds { get; set; }
    }
}

