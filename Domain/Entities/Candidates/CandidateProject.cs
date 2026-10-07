using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Candidates
{
    public class CandidateProject : BaseEntity
    {
        public Guid CandidateId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? RepositoryUrl { get; set; }

        public string? ProjectUrl { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public CandidateProfile Candidate { get; set; } = null!;
    }
}