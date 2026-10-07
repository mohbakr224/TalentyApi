using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Skills
{
        public class SkillCategory : BaseEntity
        {
            public string Name { get; set; } = null!;

            public string? Description { get; set; }

            public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        
    }
}
