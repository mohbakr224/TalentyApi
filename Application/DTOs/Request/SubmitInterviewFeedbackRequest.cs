using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class SubmitInterviewFeedbackRequest
    {
        [Required]
        public string Feedback { get; set; } = string.Empty;

        [Range(1, 5)]
        public int? Rating { get; set; }

        public string? Status { get; set; } = "COMPLETED";
    }
}

