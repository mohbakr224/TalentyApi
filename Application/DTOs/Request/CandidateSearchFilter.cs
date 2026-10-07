namespace Application.DTOs.Request
{
    public class CandidateSearchFilter
    {
        public string? Query { get; set; }
        public string? Location { get; set; }
        public string? CareerLevel { get; set; }
        public string? TargetRole { get; set; }
        public string? WorkPreference { get; set; }
        public string? Availability { get; set; }
        public decimal? MinExperience { get; set; }
        public decimal? MaxExperience { get; set; }
        public string? Skill { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; } = 0;
    }
}

