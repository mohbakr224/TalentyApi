using System;
using System.Collections.Generic;

namespace Application.DTOs.Response
{
    public class ResumeResponse
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string Status { get; set; } = string.Empty;

        public Guid? CurrentVersionId { get; set; }

        public int? CurrentVersionNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public List<ResumeVersionResponse> Versions { get; set; } = new List<ResumeVersionResponse>();
    }
}
