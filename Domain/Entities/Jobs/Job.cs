using Domain.Common;
using Domain.Entities.Companies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Jobs
{
        public class Job : BaseEntity, ISoftDeletable
        {
            public Guid CompanyId { get; set; }

            public Guid CreatedBy { get; set; }

            public string Title { get; set; } = null!;

            public string Description { get; set; } = null!;

            public string? Industry { get; set; }

            public string? Location { get; set; }

            public string? WorkType { get; set; }

            public string? ExperienceLevel { get; set; }

            public decimal? SalaryMin { get; set; }

            public decimal? SalaryMax { get; set; }

            public string? Currency { get; set; }

            public string Status { get; set; } = "DRAFT";

            public DateTime? PublishedAt { get; set; }

            public DateTime? ClosedAt { get; set; }

            public bool IsDeleted { get; set; }

            public DateTime? DeletedAt { get; set; }

            public Company Company { get; set; } = null!;

            public ICollection<JobSkill> Skills { get; set; } = new List<JobSkill>();
       
    }
}
