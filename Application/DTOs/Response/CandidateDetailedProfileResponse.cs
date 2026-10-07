using System;
using System.Collections.Generic;

namespace Application.DTOs.Response
{
    public class CandidateDetailedProfileResponse
    {
        public Guid CandidateId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Headline { get; set; }
        public string? About { get; set; }
        public string? Location { get; set; }
        public string? CareerLevel { get; set; }
        public string? TargetRole { get; set; }
        public decimal? YearsOfExperience { get; set; }
        public string? WorkPreference { get; set; }
        public string? Availability { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public bool IsPublic { get; set; }

        public List<ExperienceResponse> Experiences { get; set; } = new List<ExperienceResponse>();
        public List<EducationResponse> Education { get; set; } = new List<EducationResponse>();
        public List<SkillResponse> Skills { get; set; } = new List<SkillResponse>();
        public List<LanguageResponse> Languages { get; set; } = new List<LanguageResponse>();
        public List<ProjectResponse> Projects { get; set; } = new List<ProjectResponse>();
    }
}

