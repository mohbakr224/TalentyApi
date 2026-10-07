using Domain.Common;
using System;

namespace Domain.Entities.Candidates
{
    public class CandidateExperience : BaseEntity
    {
        public Guid CandidateId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsCurrent { get; set; } = false;

        // Navigation Property
        public CandidateProfile Candidate { get; set; } = null!;
    }
}