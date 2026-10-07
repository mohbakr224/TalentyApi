using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Candidates
{
    public class CandidateCertification : BaseEntity
    {
        public Guid CandidateId { get; set; }

        public string Name { get; set; } = null!;

        public string? IssuingOrganization { get; set; }

        public string? CredentialId { get; set; }

        public string? CredentialUrl { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public CandidateProfile Candidate { get; set; } = null!;
    }
}
