using Domain.Common;
using Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Applications
{
    public class ApplicationStatusHistory : BaseEntity
    {
        public Guid ApplicationId { get; set; }

        public string? FromStatus { get; set; }

        public string ToStatus { get; set; } = null!;

        public Guid ChangedBy { get; set; }

        public string? Reason { get; set; }

        public Application Application { get; set; } = null!;

        public User ChangedByUser { get; set; } = null!;
    }
}
