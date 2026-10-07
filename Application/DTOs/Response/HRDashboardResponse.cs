using System;

namespace Application.DTOs.Response
{
    public class HRDashboardResponse
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int DraftJobs { get; set; }
        public int ClosedJobs { get; set; }
        public int TotalApplications { get; set; }
        public int AppliedCount { get; set; }
        public int ScreeningCount { get; set; }
        public int ShortlistedCount { get; set; }
        public int InterviewCount { get; set; }
        public int OfferCount { get; set; }
        public int HiredCount { get; set; }
        public int RejectedCount { get; set; }
        public int UpcomingInterviewsCount { get; set; }
        public int TeamMembersCount { get; set; }
    }
}

