using System;
using System.Collections.Generic;

namespace Application.DTOs.Response
{
    public class CandidateSearchResponse
    {
        public Guid CandidateId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Headline { get; set; }
        public string? Location { get; set; }
        public string? CareerLevel { get; set; }
        public string? TargetRole { get; set; }
        public decimal? YearsOfExperience { get; set; }
        public string? WorkPreference { get; set; }
        public string? Availability { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
    }
}

