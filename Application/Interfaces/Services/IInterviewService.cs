using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IInterviewService
    {
        Task<InterviewResponse> ScheduleAsync(CreateInterviewRequest request, Guid interviewerId);

        Task<InterviewResponse?> GetByIdAsync(Guid interviewId);

        Task<List<InterviewResponse>> SearchAsync(InterviewSearchFilter filter, CancellationToken cancellationToken);

        Task<InterviewResponse> UpdateAsync(Guid id, UpdateInterviewRequest request, Guid userId);

        Task<InterviewResponse> CancelAsync(Guid id, Guid userId, string? reason);

        Task<InterviewResponse> SubmitFeedbackAsync(Guid id, SubmitInterviewFeedbackRequest request, Guid userId);

        // Interview Sessions
        Task<InterviewSessionResponse> CreateSessionAsync(CreateInterviewSessionRequest request, Guid userId);

        Task<InterviewSessionResponse?> GetSessionByIdAsync(Guid sessionId);

        Task<List<InterviewSessionResponse>> GetSessionsByInterviewIdAsync(Guid interviewId);

        Task<InterviewSessionResponse> UpdateSessionAsync(Guid sessionId, UpdateInterviewSessionRequest request, Guid userId);

        Task<InterviewSessionResponse> CancelSessionAsync(Guid sessionId, Guid userId, string? reason);

        // Interview Questions & Answers
        Task<InterviewQuestionResponse> AddQuestionAsync(CreateInterviewQuestionRequest request, Guid userId);

        Task<InterviewQuestionResponse?> GetQuestionByIdAsync(Guid questionId);

        Task<List<InterviewQuestionResponse>> GetQuestionsBySessionIdAsync(Guid sessionId);

        Task<InterviewQuestionResponse> UpdateQuestionAsync(Guid questionId, CreateInterviewQuestionRequest request, Guid userId);

        Task DeleteQuestionAsync(Guid questionId, Guid userId);

        Task<InterviewAnswerResponse> SubmitAnswerAsync(SubmitInterviewAnswerRequest request, Guid candidateId);

        Task<List<InterviewAnswerResponse>> GetAnswersByQuestionIdAsync(Guid questionId);

        Task<List<InterviewAnswerResponse>> GetAnswersByCandidateIdAsync(Guid candidateId);

        Task<InterviewAnswerResponse> EvaluateAnswerAsync(Guid answerId, int score, string? notes, Guid evaluatorId);

        // Interviewer Availability
        Task<InterviewerAvailabilityResponse> CreateAvailabilityAsync(CreateInterviewerAvailabilityRequest request, Guid interviewerId);

        Task<InterviewerAvailabilityResponse?> GetAvailabilityByIdAsync(Guid availabilityId);

        Task<List<InterviewerAvailabilityResponse>> GetMyAvailabilitiesAsync(Guid interviewerId, DateTime? from, DateTime? to, CancellationToken cancellationToken);

        Task<List<InterviewerAvailabilityResponse>> SearchAvailabilitiesAsync(Guid? companyId, Guid? interviewerId, Guid? jobId, DateTime? from, DateTime? to, string? status, int limit, int offset, CancellationToken cancellationToken);

        Task<InterviewerAvailabilityResponse> UpdateAvailabilityAsync(Guid availabilityId, UpdateInterviewerAvailabilityRequest request, Guid userId);

        Task DeleteAvailabilityAsync(Guid availabilityId, Guid userId);

        // Interview Bookings
        Task<InterviewBookingResponse> BookAsync(CreateInterviewBookingRequest request, Guid candidateId);

        Task<InterviewBookingResponse?> GetBookingByIdAsync(Guid bookingId);

        Task<List<InterviewBookingResponse>> GetMyBookingsAsync(Guid candidateId, string? status, CancellationToken cancellationToken);

        Task<List<InterviewBookingResponse>> GetBookingsForInterviewerAsync(Guid interviewerId, string? status, CancellationToken cancellationToken);

        Task<InterviewBookingResponse> UpdateBookingAsync(Guid bookingId, UpdateInterviewBookingRequest request, Guid userId);

        Task<InterviewBookingResponse> CancelBookingAsync(Guid bookingId, Guid userId, string? reason);

        // Interview Evaluations
        Task<InterviewEvaluationResponse> CreateEvaluationAsync(CreateInterviewEvaluationRequest request, Guid evaluatorId);

        Task<InterviewEvaluationResponse?> GetEvaluationByIdAsync(Guid evaluationId);

        Task<List<InterviewEvaluationResponse>> GetEvaluationsByInterviewIdAsync(Guid interviewId);

        Task<List<InterviewEvaluationResponse>> GetEvaluationsBySessionIdAsync(Guid sessionId);

        Task<InterviewEvaluationResponse> SubmitEvaluationAsync(Guid evaluationId, Guid evaluatorId);

        // Interview Reports
        Task<InterviewReportResponse> GenerateReportAsync(CreateInterviewReportRequest request, Guid generatedById);

        Task<InterviewReportResponse?> GetReportByIdAsync(Guid reportId);

        Task<List<InterviewReportResponse>> GetReportsByInterviewIdAsync(Guid interviewId);

        Task<InterviewReportResponse> FinalizeReportAsync(Guid reportId, Guid userId);
    }
}

