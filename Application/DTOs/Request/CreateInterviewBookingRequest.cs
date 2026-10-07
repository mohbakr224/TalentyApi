using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateInterviewBookingRequest
    {
        [Required]
        public Guid AvailabilityId { get; set; }

        public Guid? JobId { get; set; }

        public Guid? ApplicationId { get; set; }

        public string? Notes { get; set; }
    }
}