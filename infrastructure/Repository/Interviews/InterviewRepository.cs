using Application.Interfaces.Persistence;
using Domain.Entities.Interviews;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Repository.Interviews
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly AppDBcontext _context;

        public InterviewRepository(AppDBcontext context)
        {
            _context = context;
        }

        public async Task AddAsync(Interview interview)
        {
            await _context.Interviews.AddAsync(interview);
            await _context.SaveChangesAsync();
        }

        public async Task<Interview?> GetByIdAsync(Guid id)
        {
            return await _context.Interviews
                .Include(i => i.Application)
                .Include(i => i.Candidate).ThenInclude(c => c.User)
                .Include(i => i.Job)
                .Include(i => i.Company)
                .Include(i => i.Interviewer)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
        }

        public async Task UpdateAsync(Interview interview)
        {
            interview.UpdatedAt = DateTime.UtcNow;
            _context.Interviews.Update(interview);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Interview>> SearchAsync(
            Guid? companyId,
            Guid? jobId,
            Guid? applicationId,
            Guid? candidateId,
            Guid? interviewerId,
            string? status,
            DateTime? from,
            DateTime? to,
            int limit,
            int offset,
            CancellationToken cancellationToken)
        {
            var query = _context.Interviews
                .Include(i => i.Application)
                .Include(i => i.Candidate).ThenInclude(c => c.User)
                .Include(i => i.Job)
                .Include(i => i.Company)
                .Include(i => i.Interviewer)
                .Where(i => !i.IsDeleted);

            if (companyId.HasValue)
                query = query.Where(i => i.CompanyId == companyId.Value);

            if (jobId.HasValue)
                query = query.Where(i => i.JobId == jobId.Value);

            if (applicationId.HasValue)
                query = query.Where(i => i.ApplicationId == applicationId.Value);

            if (candidateId.HasValue)
                query = query.Where(i => i.CandidateId == candidateId.Value);

            if (interviewerId.HasValue)
                query = query.Where(i => i.InterviewerId == interviewerId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(i => i.Status == status);

            if (from.HasValue)
                query = query.Where(i => i.ScheduledAt >= from.Value);

            if (to.HasValue)
                query = query.Where(i => i.ScheduledAt <= to.Value);

            return await query
                .OrderBy(i => i.ScheduledAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        // Interview Sessions
        public async Task AddSessionAsync(InterviewSession session)
        {
            await _context.InterviewSessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }

        public async Task<InterviewSession?> GetSessionByIdAsync(Guid sessionId)
        {
            return await _context.InterviewSessions
                .Include(s => s.Interview)
                .Include(s => s.Interviewer)
                .Include(s => s.Questions)
                .Include(s => s.Evaluations)
                .FirstOrDefaultAsync(s => s.Id == sessionId && !s.IsDeleted);
        }

        public async Task UpdateSessionAsync(InterviewSession session)
        {
            session.UpdatedAt = DateTime.UtcNow;
            _context.InterviewSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InterviewSession>> GetSessionsByInterviewIdAsync(Guid interviewId)
        {
            return await _context.InterviewSessions
                .Include(s => s.Interviewer)
                .Include(s => s.Questions)
                .Include(s => s.Evaluations)
                .Where(s => s.InterviewId == interviewId && !s.IsDeleted)
                .OrderBy(s => s.SessionNumber)
                .ToListAsync();
        }

        // Interview Questions & Answers
        public async Task AddQuestionAsync(InterviewQuestion question)
        {
            await _context.InterviewQuestions.AddAsync(question);
            await _context.SaveChangesAsync();
        }

        public async Task<InterviewQuestion?> GetQuestionByIdAsync(Guid questionId)
        {
            return await _context.InterviewQuestions
                .Include(q => q.Session)
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == questionId && !q.IsDeleted);
        }

        public async Task UpdateQuestionAsync(InterviewQuestion question)
        {
            question.UpdatedAt = DateTime.UtcNow;
            _context.InterviewQuestions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(Guid questionId)
        {
            var question = await _context.InterviewQuestions.FindAsync(questionId);
            if (question != null)
            {
                question.IsDeleted = true;
                question.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<InterviewQuestion>> GetQuestionsBySessionIdAsync(Guid sessionId)
        {
            return await _context.InterviewQuestions
                .Include(q => q.Answers)
                .Where(q => q.SessionId == sessionId && !q.IsDeleted)
                .OrderBy(q => q.Order)
                .ToListAsync();
        }

        public async Task AddAnswerAsync(InterviewAnswer answer)
        {
            await _context.InterviewAnswers.AddAsync(answer);
            await _context.SaveChangesAsync();
        }

        public async Task<InterviewAnswer?> GetAnswerByIdAsync(Guid answerId)
        {
            return await _context.InterviewAnswers
                .Include(a => a.Question)
                .Include(a => a.Candidate)
                .Include(a => a.EvaluatedBy)
                .FirstOrDefaultAsync(a => a.Id == answerId && !a.IsDeleted);
        }

        public async Task UpdateAnswerAsync(InterviewAnswer answer)
        {
            answer.UpdatedAt = DateTime.UtcNow;
            _context.InterviewAnswers.Update(answer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InterviewAnswer>> GetAnswersByQuestionIdAsync(Guid questionId)
        {
            return await _context.InterviewAnswers
                .Include(a => a.Candidate)
                .Include(a => a.EvaluatedBy)
                .Where(a => a.QuestionId == questionId && !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<InterviewAnswer>> GetAnswersByCandidateIdAsync(Guid candidateId)
        {
            return await _context.InterviewAnswers
                .Include(a => a.Question).ThenInclude(q => q.Session)
                .Include(a => a.EvaluatedBy)
                .Where(a => a.CandidateId == candidateId && !a.IsDeleted)
                .ToListAsync();
        }

        // Interviewer Availability
        public async Task AddAvailabilityAsync(InterviewerAvailability availability)
        {
            await _context.InterviewerAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
        }

        public async Task<InterviewerAvailability?> GetAvailabilityByIdAsync(Guid availabilityId)
        {
            return await _context.InterviewerAvailabilities
                .Include(a => a.Interviewer)
                .Include(a => a.Job)
                .Include(a => a.Company)
                .Include(a => a.Bookings)
                .FirstOrDefaultAsync(a => a.Id == availabilityId && !a.IsDeleted);
        }

        public async Task UpdateAvailabilityAsync(InterviewerAvailability availability)
        {
            availability.UpdatedAt = DateTime.UtcNow;
            _context.InterviewerAvailabilities.Update(availability);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAvailabilityAsync(Guid availabilityId)
        {
            var availability = await _context.InterviewerAvailabilities.FindAsync(availabilityId);
            if (availability != null)
            {
                availability.IsDeleted = true;
                availability.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<InterviewerAvailability>> GetAvailabilitiesByInterviewerIdAsync(Guid interviewerId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var query = _context.InterviewerAvailabilities
                .Include(a => a.Job)
                .Include(a => a.Company)
                .Include(a => a.Bookings)
                .Where(a => a.InterviewerId == interviewerId && !a.IsDeleted);

            if (from.HasValue)
                query = query.Where(a => a.EndTime >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.StartTime <= to.Value);

            return await query.OrderBy(a => a.StartTime).ToListAsync(cancellationToken);
        }

        public async Task<List<InterviewerAvailability>> GetAvailabilitiesByJobIdAsync(Guid jobId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var query = _context.InterviewerAvailabilities
                .Include(a => a.Interviewer)
                .Include(a => a.Company)
                .Include(a => a.Bookings)
                .Where(a => a.JobId == jobId && !a.IsDeleted);

            if (from.HasValue)
                query = query.Where(a => a.EndTime >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.StartTime <= to.Value);

            return await query.OrderBy(a => a.StartTime).ToListAsync(cancellationToken);
        }

        public async Task<List<InterviewerAvailability>> SearchAvailabilitiesAsync(Guid? companyId, Guid? interviewerId, Guid? jobId, DateTime? from, DateTime? to, string? status, int limit, int offset, CancellationToken cancellationToken)
        {
            var query = _context.InterviewerAvailabilities
                .Include(a => a.Interviewer)
                .Include(a => a.Job)
                .Include(a => a.Company)
                .Include(a => a.Bookings)
                .Where(a => !a.IsDeleted);

            if (companyId.HasValue)
                query = query.Where(a => a.CompanyId == companyId.Value);

            if (interviewerId.HasValue)
                query = query.Where(a => a.InterviewerId == interviewerId.Value);

            if (jobId.HasValue)
                query = query.Where(a => a.JobId == jobId.Value);

            if (from.HasValue)
                query = query.Where(a => a.EndTime >= from.Value);

            if (to.HasValue)
                query = query.Where(a => a.StartTime <= to.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            return await query
                .OrderBy(a => a.StartTime)
                .Skip(offset)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }

        // Interview Bookings
        public async Task AddBookingAsync(InterviewBooking booking)
        {
            await _context.InterviewBookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<InterviewBooking?> GetBookingByIdAsync(Guid bookingId)
        {
            return await _context.InterviewBookings
                .Include(b => b.Availability).ThenInclude(a => a.Interviewer)
                .Include(b => b.Candidate)
                .Include(b => b.Job)
                .Include(b => b.Application)
                .Include(b => b.CancelledBy)
                .Include(b => b.CreatedInterview)
                .FirstOrDefaultAsync(b => b.Id == bookingId && !b.IsDeleted);
        }

        public async Task UpdateBookingAsync(InterviewBooking booking)
        {
            booking.UpdatedAt = DateTime.UtcNow;
            _context.InterviewBookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InterviewBooking>> GetBookingsByAvailabilityIdAsync(Guid availabilityId)
        {
            return await _context.InterviewBookings
                .Include(b => b.Candidate)
                .Include(b => b.Job)
                .Include(b => b.Application)
                .Where(b => b.AvailabilityId == availabilityId && !b.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<InterviewBooking>> GetBookingsByCandidateIdAsync(Guid candidateId, string? status, CancellationToken cancellationToken)
        {
            var query = _context.InterviewBookings
                .Include(b => b.Availability).ThenInclude(a => a.Interviewer)
                .Include(b => b.Job)
                .Include(b => b.Application)
                .Where(b => b.CandidateId == candidateId && !b.IsDeleted);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(b => b.Status == status);

            return await query.OrderBy(b => b.ScheduledAt).ToListAsync(cancellationToken);
        }

        public async Task<List<InterviewBooking>> GetBookingsByInterviewerIdAsync(Guid interviewerId, string? status, CancellationToken cancellationToken)
        {
            var query = _context.InterviewBookings
                .Include(b => b.Availability)
                .Include(b => b.Candidate)
                .Include(b => b.Job)
                .Include(b => b.Application)
                .Where(b => b.Availability.InterviewerId == interviewerId && !b.IsDeleted);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(b => b.Status == status);

            return await query.OrderBy(b => b.ScheduledAt).ToListAsync(cancellationToken);
        }

        public async Task<bool> HasConflictingBookingAsync(Guid interviewerId, DateTime scheduledAt, int durationMinutes, Guid? excludeBookingId, CancellationToken cancellationToken)
        {
            var endTime = scheduledAt.AddMinutes(durationMinutes);

            var query = _context.InterviewBookings
                .Include(b => b.Availability)
                .Where(b => b.Availability.InterviewerId == interviewerId
                    && !b.IsDeleted
                    && b.Status != "CANCELLED_BY_CANDIDATE"
                    && b.Status != "CANCELLED_BY_INTERVIEWER"
                    && b.ScheduledAt < endTime
                    && b.ScheduledAt.AddMinutes(b.DurationMinutes) > scheduledAt);

            if (excludeBookingId.HasValue)
                query = query.Where(b => b.Id != excludeBookingId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        // Interview Evaluations
        public async Task AddEvaluationAsync(InterviewEvaluation evaluation)
        {
            await _context.InterviewEvaluations.AddAsync(evaluation);
            await _context.SaveChangesAsync();
        }

        public async Task<InterviewEvaluation?> GetEvaluationByIdAsync(Guid evaluationId)
        {
            return await _context.InterviewEvaluations
                .Include(e => e.Session)
                .Include(e => e.Interview)
                .Include(e => e.Evaluator)
                .Include(e => e.Candidate)
                .Include(e => e.CriteriaScores)
                .FirstOrDefaultAsync(e => e.Id == evaluationId && !e.IsDeleted);
        }

        public async Task UpdateEvaluationAsync(InterviewEvaluation evaluation)
        {
            evaluation.UpdatedAt = DateTime.UtcNow;
            _context.InterviewEvaluations.Update(evaluation);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InterviewEvaluation>> GetEvaluationsByInterviewIdAsync(Guid interviewId)
        {
            return await _context.InterviewEvaluations
                .Include(e => e.Session)
                .Include(e => e.Evaluator)
                .Include(e => e.CriteriaScores)
                .Where(e => e.InterviewId == interviewId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<InterviewEvaluation>> GetEvaluationsBySessionIdAsync(Guid sessionId)
        {
            return await _context.InterviewEvaluations
                .Include(e => e.Evaluator)
                .Include(e => e.CriteriaScores)
                .Where(e => e.SessionId == sessionId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task AddEvaluationCriteriaAsync(InterviewEvaluationCriteria criteria)
        {
            await _context.InterviewEvaluationCriteria.AddAsync(criteria);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InterviewEvaluationCriteria>> GetCriteriaByEvaluationIdAsync(Guid evaluationId)
        {
            return await _context.InterviewEvaluationCriteria
                .Where(c => c.EvaluationId == evaluationId && !c.IsDeleted)
                .ToListAsync();
        }

        // Interview Reports
        public async Task AddReportAsync(InterviewReport report)
        {
            await _context.InterviewReports.AddAsync(report);
            await _context.SaveChangesAsync();
        }

        public async Task<InterviewReport?> GetReportByIdAsync(Guid reportId)
        {
            return await _context.InterviewReports
                .Include(r => r.Interview)
                .Include(r => r.Candidate)
                .Include(r => r.GeneratedBy)
                .FirstOrDefaultAsync(r => r.Id == reportId && !r.IsDeleted);
        }

        public async Task UpdateReportAsync(InterviewReport report)
        {
            report.UpdatedAt = DateTime.UtcNow;
            _context.InterviewReports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InterviewReport>> GetReportsByInterviewIdAsync(Guid interviewId)
        {
            return await _context.InterviewReports
                .Include(r => r.Candidate)
                .Include(r => r.GeneratedBy)
                .Where(r => r.InterviewId == interviewId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<InterviewReport?> GetLatestReportByInterviewIdAsync(Guid interviewId)
        {
            return await _context.InterviewReports
                .Include(r => r.Candidate)
                .Include(r => r.GeneratedBy)
                .Where(r => r.InterviewId == interviewId && !r.IsDeleted)
                .OrderByDescending(r => r.CreatedDate)
                .FirstOrDefaultAsync();
        }
    }
}