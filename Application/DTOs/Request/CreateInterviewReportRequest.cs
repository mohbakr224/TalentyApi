using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateInterviewReportRequest
    {
        [Required]
        public Guid InterviewId { get; set; }

        [Required]
        public string ReportType { get; set; } = "COMPREHENSIVE"; // COMPREHENSIVE, SUMMARY, TECHNICAL, BEHAVIORAL

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty; // JSON or Markdown

        public string? ExecutiveSummary { get; set; }

        public string? FinalRecommendation { get; set; } // HIRE, NO_HIRE, STRONG_HIRE, STRONG_NO_HIRE, NEED_MORE_INFO

        public int? OverallScore { get; set; }
    }
}