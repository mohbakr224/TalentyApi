using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Resumes
{
        public class ResumeVersion : BaseEntity
        {
            public Guid ResumeId { get; set; }

            public int VersionNumber { get; set; }

            public string? ExtractedText { get; set; }

            public string? StructuredData { get; set; }

            public Resume Resume { get; set; } = null!;
        }
    
}
