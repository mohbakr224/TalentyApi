using Domain.Common;
using Domain.Entities.Applications;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Candidates
{
    public class CandidateProfile : BaseEntity, ISoftDeletable
    {
        public Guid UserId { get; set; }

        public string? Headline { get; set; }

        public string? About { get; set; }

        public string? Location { get; set; }

        public string? CareerLevel { get; set; }

        public string? TargetRole { get; set; }

        public decimal? YearsOfExperience { get; set; }

        public string? WorkPreference { get; set; }

        public string? Availability { get; set; }

        public decimal? SalaryExpectation { get; set; }

        public bool IsPublic { get; set; } = true;

        // Soft Delete
        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;

        public ICollection<CandidateExperience> Experiences { get; set; }
            = new List<CandidateExperience>();

        public ICollection<CandidateEducation> Education { get; set; }
            = new List<CandidateEducation>();

        public ICollection<CandidateProject> Projects { get; set; }
            = new List<CandidateProject>();

        public ICollection<CandidateCertification> Certifications { get; set; }
            = new List<CandidateCertification>();

        public ICollection<CandidateLanguage> Languages { get; set; }
            = new List<CandidateLanguage>();

        public ICollection<CandidateSkill> Skills { get; set; }
            = new List<CandidateSkill>();

        public ICollection<Domain.Entities.Applications.Application> Applications { get; set; }
            = new List<Domain.Entities.Applications.Application>();
    }
}