using System;

namespace Application.DTOs.Response
{
    public class InterviewAnswerResponse
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public Guid CandidateId { get; set; }
        public string CandidateName { get; set; } = string.Empty;
        public string AnswerText { get; set; } = string.Empty;
        public int? Score { get; set; }
        public string? EvaluatorNotes { get; set; }
        public Guid? EvaluatedById { get; set; }
        public string? EvaluatedByName { get; set; }
        public DateTime? EvaluatedAt { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}