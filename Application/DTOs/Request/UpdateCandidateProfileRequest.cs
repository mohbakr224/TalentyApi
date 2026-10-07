using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class UpdateCandidateProfileRequest
    {
        [MaxLength(250)]
        public string? Headline { get; set; }

        [MaxLength(2000)]
        public string? About { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public string? CareerLevel { get; set; }

        public string? TargetRole { get; set; }

        public decimal? YearsOfExperience { get; set; }

        public string? WorkPreference { get; set; }

        public string? Availability { get; set; }

        public decimal? SalaryExpectation { get; set; }

        public bool? IsPublic { get; set; }
    }
}
