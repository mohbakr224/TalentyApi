using System;

namespace Application.DTOs.Response
{
    public class ResumeListItemResponse
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string Status { get; set; } = string.Empty;

        public Guid? CurrentVersionId { get; set; }

        public int? CurrentVersionNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
