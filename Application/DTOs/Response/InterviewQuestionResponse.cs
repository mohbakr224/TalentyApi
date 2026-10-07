using System;

namespace Application.DTOs.Response
{
    public class InterviewQuestionResponse
    {
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public int Order { get; set; }
        public int? MaxScore { get; set; }
        public string? ExpectedAnswer { get; set; }
        public string? Category { get; set; }
        public string? Difficulty { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}