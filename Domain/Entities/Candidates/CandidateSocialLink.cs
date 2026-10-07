using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Candidates
{
    public class CandidateSocialLink : BaseEntity
    {
        public Guid CandidateId { get; set; }

        public string Platform { get; set; } = null!;

        public string Url { get; set; } = null!;

        public CandidateProfile Candidate { get; set; } = null!;
    }
}
