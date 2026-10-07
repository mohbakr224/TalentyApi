using Domain.Common;
using Domain.Entities.Candidates;
using Domain.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Skills
{
        public class Skill : BaseEntity
        {
            public Guid CategoryId { get; set; }

            public string Name { get; set; } = null!;

            public string? Description { get; set; }

            public SkillCategory Category { get; set; } = null!;

            public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();

            public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
        
    }
}
