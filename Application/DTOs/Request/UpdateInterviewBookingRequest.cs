using System;

namespace Application.DTOs.Request
{
    public class UpdateInterviewBookingRequest
    {
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string? LocationOrLink { get; set; }
    }
}