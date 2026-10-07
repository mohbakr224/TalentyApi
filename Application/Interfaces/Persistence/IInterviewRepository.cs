using Domain.Entities.Interviews;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface IInterviewRepository
    {
        Task AddAsync(Interview interview);
        Task<Interview?> GetByIdAsync(Guid id);
        Task UpdateAsync(Interview interview);
        Task<List<Interview>> SearchAsync(Guid? companyId, Guid? jobId, Guid? applicationId, Guid? candidateId, Guid? interviewerId, string? status, DateTime? from, DateTime? to, int limit, int offset, CancellationToken cancellationToken);

        // Interview Sessions
        Task AddSessionAsync(InterviewSession session);
        Task<InterviewSession?> GetSessionByIdAsync(Guid sessionId);
        Task UpdateSessionAsync(InterviewSession session);
        Task<List<InterviewSession>> GetSessionsByInterviewIdAsync(Guid interviewId);

        // Interview Questions & Answers
        Task AddQuestionAsync(InterviewQuestion question);
        Task<InterviewQuestion?> GetQuestionByIdAsync(Guid questionId);
        Task UpdateQuestionAsync(InterviewQuestion question);
        Task DeleteQuestionAsync(Guid questionId);
        Task<List<InterviewQuestion>> GetQuestionsBySessionIdAsync(Guid sessionId);
        Task AddAnswerAsync(InterviewAnswer answer);
        Task<InterviewAnswer?> GetAnswerByIdAsync(Guid answerId);
        Task UpdateAnswerAsync(InterviewAnswer answer);
        Task<List<InterviewAnswer>> GetAnswersByQuestionIdAsync(Guid questionId);
        Task<List<InterviewAnswer>> GetAnswersByCandidateIdAsync(Guid candidateId);

        // Interviewer Availability
        Task AddAvailabilityAsync(InterviewerAvailability availability);
        Task<InterviewerAvailability?> GetAvailabilityByIdAsync(Guid availabilityId);
        Task UpdateAvailabilityAsync(InterviewerAvailability availability);
        Task DeleteAvailabilityAsync(Guid availabilityId);
        Task<List<InterviewerAvailability>> GetAvailabilitiesByInterviewerIdAsync(Guid interviewerId, DateTime? from, DateTime? to, CancellationToken cancellationToken);
        Task<List<InterviewerAvailability>> GetAvailabilitiesByJobIdAsync(Guid jobId, DateTime? from, DateTime? to, CancellationToken cancellationToken);
        Task<List<InterviewerAvailability>> SearchAvailabilitiesAsync(Guid? companyId, Guid? interviewerId, Guid? jobId, DateTime? from, DateTime? to, string? status, int limit, int offset, CancellationToken cancellationToken);

        // Interview Bookings
        Task AddBookingAsync(InterviewBooking booking);
        Task<InterviewBooking?> GetBookingByIdAsync(Guid bookingId);
        Task UpdateBookingAsync(InterviewBooking booking);
        Task<List<InterviewBooking>> GetBookingsByAvailabilityIdAsync(Guid availabilityId);
        Task<List<InterviewBooking>> GetBookingsByCandidateIdAsync(Guid candidateId, string? status, CancellationToken cancellationToken);
        Task<List<InterviewBooking>> GetBookingsByInterviewerIdAsync(Guid interviewerId, string? status, CancellationToken cancellationToken);
        Task<bool> HasConflictingBookingAsync(Guid interviewerId, DateTime scheduledAt, int durationMinutes, Guid? excludeBookingId, CancellationToken cancellationToken);

        // Interview Evaluations
        Task AddEvaluationAsync(InterviewEvaluation evaluation);
        Task<InterviewEvaluation?> GetEvaluationByIdAsync(Guid evaluationId);
        Task UpdateEvaluationAsync(InterviewEvaluation evaluation);
        Task<List<InterviewEvaluation>> GetEvaluationsByInterviewIdAsync(Guid interviewId);
        Task<List<InterviewEvaluation>> GetEvaluationsBySessionIdAsync(Guid sessionId);
        Task AddEvaluationCriteriaAsync(InterviewEvaluationCriteria criteria);
        Task<List<InterviewEvaluationCriteria>> GetCriteriaByEvaluationIdAsync(Guid evaluationId);

        // Interview Reports
        Task AddReportAsync(InterviewReport report);
        Task<InterviewReport?> GetReportByIdAsync(Guid reportId);
        Task UpdateReportAsync(InterviewReport report);
        Task<List<InterviewReport>> GetReportsByInterviewIdAsync(Guid interviewId);
        Task<InterviewReport?> GetLatestReportByInterviewIdAsync(Guid interviewId);
    }
}

