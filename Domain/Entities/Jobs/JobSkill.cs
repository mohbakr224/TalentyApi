using Domain.Entities.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Jobs
{
        public class JobSkill
        {
            public Guid JobId { get; set; }

            public Guid SkillId { get; set; }

            public bool Required { get; set; }

            public string? ProficiencyLevel { get; set; }

            public Job Job { get; set; } = null!;

            public Skill Skill { get; set; } = null!;
    }
}
