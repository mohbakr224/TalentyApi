using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateApplicationRequest
    {
        [Required]
        public Guid JobId { get; set; }

        public Guid? ResumeId { get; set; }

        [MaxLength(2000)]
        public string? CoverLetter { get; set; }
    }
}
