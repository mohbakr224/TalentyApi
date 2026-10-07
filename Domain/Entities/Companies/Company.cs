using Domain.Common;
using Domain.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Companies
{
    public class Company : BaseEntity, ISoftDeletable
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Industry { get; set; }

        public string? Website { get; set; }

        public string? Location { get; set; }

        public Guid? LogoFileId { get; set; }

        public string? VerificationStatus { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public ICollection<CompanyMember> Members { get; set; } = new List<CompanyMember>();

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
