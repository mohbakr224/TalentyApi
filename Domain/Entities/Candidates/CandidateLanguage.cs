using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Candidates
{
    public class CandidateLanguage : BaseEntity
    {
        public Guid CandidateId { get; set; }

        public string Language { get; set; } = null!;

        public string ProficiencyLevel { get; set; } = null!;

        public CandidateProfile Candidate { get; set; } = null!;
    }
}
