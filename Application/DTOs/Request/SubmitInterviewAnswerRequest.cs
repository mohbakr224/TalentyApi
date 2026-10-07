using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class SubmitInterviewAnswerRequest
    {
        [Required]
        public Guid QuestionId { get; set; }

        [Required]
        public string AnswerText { get; set; } = string.Empty;
    }
}