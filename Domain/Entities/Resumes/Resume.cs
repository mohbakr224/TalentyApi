using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Resumes
{
    public class Resume : BaseEntity, ISoftDeletable
    {
        public Guid CandidateId { get; set; }

        public Guid FileId { get; set; }

        public string? Name { get; set; }

        public string Status { get; set; } = "UPLOADING";

        public Guid? CurrentVersionId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }
        public ICollection<ResumeVersion> versions { get; set; } = new List<ResumeVersion>();
    }
}
