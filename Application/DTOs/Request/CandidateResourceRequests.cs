using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class ExperienceRequest
    {
        [Required, MaxLength(200)] public string CompanyName { get; set; } = string.Empty;
        [Required, MaxLength(200)] public string JobTitle { get; set; } = string.Empty;
        [MaxLength(4000)] public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class EducationRequest
    {
        [Required, MaxLength(250)] public string Institution { get; set; } = string.Empty;
        [MaxLength(200)] public string? Degree { get; set; }
        [MaxLength(200)] public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? GPA { get; set; }
    }

    public class SkillRequest
    {
        [Required] public Guid SkillId { get; set; }
        [MaxLength(50)] public string? ProficiencyLevel { get; set; }
        public decimal? YearsOfExperience { get; set; }
    }

    public class LanguageRequest
    {
        [Required, MaxLength(100)] public string Language { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string ProficiencyLevel { get; set; } = string.Empty;
    }

    public class ProjectRequest
    {
        [Required, MaxLength(250)] public string Name { get; set; } = string.Empty;
        [MaxLength(4000)] public string? Description { get; set; }
        [Url, MaxLength(1000)] public string? RepositoryUrl { get; set; }
        [Url, MaxLength(1000)] public string? ProjectUrl { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
