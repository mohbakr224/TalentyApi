using System;
using System.Collections.Generic;

namespace Application.DTOs.Response
{
    public class ApplicationResponse
    {
        public Guid Id { get; set; }
        public Guid CandidateId { get; set; }
        public Guid JobId { get; set; }
        public Guid? ResumeId { get; set; }
        public string? ResumeName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CoverLetter { get; set; }
        public string? CandidateName { get; set; }
        public string? CandidateEmail { get; set; }
        public string? CandidateHeadline { get; set; }
        public string? JobTitle { get; set; }
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ApplicationStatusHistoryResponse> StatusHistory { get; set; } = new List<ApplicationStatusHistoryResponse>();
    }

    public class ApplicationStatusHistoryResponse
    {
        public Guid Id { get; set; }
        public string? FromStatus { get; set; }
        public string ToStatus { get; set; } = string.Empty;
        public Guid ChangedBy { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
