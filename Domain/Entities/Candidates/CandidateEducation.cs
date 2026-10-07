using Domain.Common;
using System;

namespace Domain.Entities.Candidates
{
    public class CandidateEducation : BaseEntity
    {
        public Guid CandidateId { get; set; }

        public string Institution { get; set; } = string.Empty;

        public string? Degree { get; set; }

        public string? FieldOfStudy { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public decimal? GPA { get; set; }

        // Navigation Property
        public CandidateProfile Candidate { get; set; } = null!;
    }
}