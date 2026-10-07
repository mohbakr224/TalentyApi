using System;

namespace Application.DTOs.Request
{
    public class InterviewSearchFilter
    {
        public Guid? CompanyId { get; set; }
        public Guid? JobId { get; set; }
        public Guid? ApplicationId { get; set; }
        public Guid? CandidateId { get; set; }
        public Guid? InterviewerId { get; set; }
        public string? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; } = 0;
    }
}

