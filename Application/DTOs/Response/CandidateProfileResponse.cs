using System;
using System.Collections.Generic;

namespace Application.DTOs.Response
{
    public class CandidateProfileResponse
    {
        public Guid UserId { get; set; }
        public string? Headline { get; set; }
        public string? About { get; set; }
        public string? Location { get; set; }
        public string? CareerLevel { get; set; }
        public string? TargetRole { get; set; }
        public decimal? YearsOfExperience { get; set; }
        public string? WorkPreference { get; set; }
        public string? Availability { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public bool IsPublic { get; set; }
    }
}