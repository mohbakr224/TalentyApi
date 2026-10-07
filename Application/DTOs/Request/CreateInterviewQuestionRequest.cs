using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Request
{
    public class CreateInterviewQuestionRequest
    {
        [Required]
        public Guid SessionId { get; set; }

        [Required]
        public string QuestionText { get; set; } = string.Empty;

        public string QuestionType { get; set; } = "OpenEnded"; // OpenEnded, MultipleChoice, Coding, TrueFalse, Scale

        public int Order { get; set; }

        public int? MaxScore { get; set; }

        public string? ExpectedAnswer { get; set; }

        public string? Category { get; set; } // Technical, Behavioral, Cultural, DomainKnowledge

        public string? Difficulty { get; set; } // Easy, Medium, Hard
    }
}