using Domain.Common;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Companies
{
    public class CompanyMember : BaseEntity
    {
        public Guid CompanyId { get; set; }

        public Guid UserId { get; set; }

        public string Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public Company Company { get; set; } = null!;

        public User User { get; set; } = null!;
    }
}
