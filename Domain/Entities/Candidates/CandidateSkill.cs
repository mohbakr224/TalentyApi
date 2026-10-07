using Domain.Entities.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Candidates
{
    public class CandidateSkill
    {
        public Guid CandidateId { get; set; }

        public Guid SkillId { get; set; }

        public string? ProficiencyLevel { get; set; }

        public decimal? YearsOfExperience { get; set; }

        public CandidateProfile Candidate { get; set; } = null!;

        public Skill Skill { get; set; } = null!;
    }
}
