using System;

namespace Application.DTOs.Response
{
    public class ExperienceResponse : Application.DTOs.Request.ExperienceRequest
    {
        public Guid Id { get; set; }
    }

    public class EducationResponse : Application.DTOs.Request.EducationRequest
    {
        public Guid Id { get; set; }
    }

    public class SkillResponse : Application.DTOs.Request.SkillRequest
    {
        public string? SkillName { get; set; }
    }

    public class LanguageResponse : Application.DTOs.Request.LanguageRequest
    {
        public Guid Id { get; set; }
    }

    public class ProjectResponse : Application.DTOs.Request.ProjectRequest
    {
        public Guid Id { get; set; }
    }
}
