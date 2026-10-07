using System;

namespace Application.DTOs.Response
{
    public class ResumeVersionResponse
    {
        public Guid Id { get; set; }

        public int VersionNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ExtractedText { get; set; }

        public string? StructuredData { get; set; }

        public bool IsCurrent { get; set; }
    }
}
