using Domain.Common;
using Domain.Entities.Candidates;
using Domain.Entities.Jobs;
using Domain.Entities.Resumes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Applications
{
    public class Application : BaseEntity
    {
        public Guid CandidateId { get; set; }

        public Guid JobId { get; set; }

        public Guid? ResumeId { get; set; }

        public string Status { get; set; } = "APPLIED";

        public string? CoverLetter { get; set; }

        public CandidateProfile Candidate { get; set; } = null!;

        public Job Job { get; set; } = null!;

        public Resume? Resume { get; set; }

        public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
    }
}
