using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities.Interviews;
using Domain.Entities.Notifications;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Services.Interviews
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly AppDBcontext _dbContext;

        public InterviewService(
            IInterviewRepository interviewRepository,
            IApplicationRepository applicationRepository,
            AppDBcontext dbContext)
        {
            _interviewRepository = interviewRepository;
            _applicationRepository = applicationRepository;
            _dbContext = dbContext;
        }

        public async Task<InterviewResponse> ScheduleAsync(CreateInterviewRequest request, Guid interviewerId)
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (application == null)
                throw new InvalidOperationException("Application not found");

            var candidateProfile = await _dbContext.CandidateProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == application.CandidateId);

            if (candidateProfile == null)
                throw new InvalidOperationException("Candidate profile not found");

            var job = await _dbContext.Jobs
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == application.JobId);

            if (job == null)
                throw new InvalidOperationException("Job not found");

            var interview = new Interview
            {
                Id = Guid.NewGuid(),
                ApplicationId = application.Id,
                CandidateId = candidateProfile.Id,
                JobId = job.Id,
                CompanyId = job.CompanyId,
                InterviewerId = interviewerId,
                Title = string.IsNullOrWhiteSpace(request.Title) ? $"Interview for {job.Title}" : request.Title,
                InterviewType = string.IsNullOrWhiteSpace(request.InterviewType) ? "HR" : request.InterviewType,
                ScheduledAt = request.ScheduledAt,
                DurationMinutes = request.DurationMinutes > 0 ? request.DurationMinutes : 30,
                LocationOrLink = request.LocationOrLink,
                Status = "SCHEDULED",
                Notes = request.Notes,
                CreatedDate = DateTime.UtcNow
            };

            await _interviewRepository.AddAsync(interview);

            // Automatically advance application status to INTERVIEW if applicable
            if (application.Status != "INTERVIEW" && application.Status != "OFFER" && application.Status != "HIRED")
            {
                application.Status = "INTERVIEW";
                application.StatusHistory.Add(new Domain.Entities.Applications.ApplicationStatusHistory
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = application.Id,
                    FromStatus = application.Status,
                    ToStatus = "INTERVIEW",
                    ChangedBy = interviewerId,
                    Reason = $"Interview scheduled: {interview.Title}",
                    CreatedDate = DateTime.UtcNow
                });
                await _applicationRepository.UpdateAsync(application);
            }

            // Send notification to candidate
            try
            {
                var candidateUserId = candidateProfile.UserId;
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = candidateUserId,
                    SenderId = interviewerId,
                    CompanyId = job.CompanyId,
                    Title = $"Interview Scheduled: {interview.Title}",
                    Message = $"An interview has been scheduled for '{job.Title}' at {job.Company?.Name} on {request.ScheduledAt:yyyy-MM-dd HH:mm UTC}.{(string.IsNullOrWhiteSpace(request.LocationOrLink) ? "" : $" Location/Link: {request.LocationOrLink}")}",
                    Type = "INTERVIEW_INVITATION",
                    RelatedEntityId = interview.Id,
                    IsRead = false,
                    CreatedDate = DateTime.UtcNow
                };
                _dbContext.Notifications.Add(notification);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                // Non-fatal
            }

            var loaded = await _interviewRepository.GetByIdAsync(interview.Id);
            return MapToResponse(loaded ?? interview);
        }

        public async Task<InterviewResponse?> GetByIdAsync(Guid interviewId)
        {
            var interview = await _interviewRepository.GetByIdAsync(interviewId);
            if (interview == null) return null;
            return MapToResponse(interview);
        }

        public async Task<List<InterviewResponse>> SearchAsync(InterviewSearchFilter filter, CancellationToken cancellationToken)
        {
            var list = await _interviewRepository.SearchAsync(
                filter.CompanyId,
                filter.JobId,
                filter.ApplicationId,
                filter.CandidateId,
                filter.InterviewerId,
                filter.Status,
                filter.From,
                filter.To,
                filter.Limit,
                filter.Offset,
                cancellationToken);

            return list.Select(MapToResponse).ToList();
        }

        public async Task<InterviewResponse> UpdateAsync(Guid id, UpdateInterviewRequest request, Guid userId)
        {
            var interview = await _interviewRepository.GetByIdAsync(id);
            if (interview == null)
                throw new InvalidOperationException("Interview not found");

            if (!string.IsNullOrWhiteSpace(request.Title)) interview.Title = request.Title;
            if (!string.IsNullOrWhiteSpace(request.InterviewType)) interview.InterviewType = request.InterviewType;
            if (request.ScheduledAt.HasValue) interview.ScheduledAt = request.ScheduledAt.Value;
            if (request.DurationMinutes.HasValue) interview.DurationMinutes = request.DurationMinutes.Value;
            if (request.LocationOrLink != null) interview.LocationOrLink = request.LocationOrLink;
            if (!string.IsNullOrWhiteSpace(request.Status)) interview.Status = request.Status;
            if (request.Notes != null) interview.Notes = request.Notes;

            await _interviewRepository.UpdateAsync(interview);

            // Notify candidate of update/reschedule
            try
            {
                var candidateUserId = interview.Candidate?.UserId;
                if (candidateUserId.HasValue)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = candidateUserId.Value,
                        SenderId = userId,
                        CompanyId = interview.CompanyId,
                        Title = $"Interview Updated: {interview.Title}",
                        Message = $"Your interview '{interview.Title}' has been updated to {interview.ScheduledAt:yyyy-MM-dd HH:mm UTC}.{(string.IsNullOrWhiteSpace(interview.LocationOrLink) ? "" : $" Location/Link: {interview.LocationOrLink}")}",
                        Type = "INTERVIEW_INVITATION",
                        RelatedEntityId = interview.Id,
                        IsRead = false,
                        CreatedDate = DateTime.UtcNow
                    };
                    _dbContext.Notifications.Add(notification);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch
            {
                // Non-fatal
            }

            return MapToResponse(interview);
        }

        public async Task<InterviewResponse> CancelAsync(Guid id, Guid userId, string? reason)
        {
            var interview = await _interviewRepository.GetByIdAsync(id);
            if (interview == null)
                throw new InvalidOperationException("Interview not found");

            interview.Status = "CANCELLED";
            if (!string.IsNullOrWhiteSpace(reason))
            {
                interview.Notes = string.IsNullOrWhiteSpace(interview.Notes)
                    ? $"Cancelled: {reason}"
                    : $"{interview.Notes} | Cancelled: {reason}";
            }

            await _interviewRepository.UpdateAsync(interview);

            // Notify candidate
            try
            {
                var candidateUserId = interview.Candidate?.UserId;
                if (candidateUserId.HasValue)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = candidateUserId.Value,
                        SenderId = userId,
                        CompanyId = interview.CompanyId,
                        Title = $"Interview Cancelled: {interview.Title}",
                        Message = $"Your interview '{interview.Title}' has been cancelled.{(string.IsNullOrWhiteSpace(reason) ? "" : $" Reason: {reason}")}",
                        Type = "INTERVIEW_INVITATION",
                        RelatedEntityId = interview.Id,
                        IsRead = false,
                        CreatedDate = DateTime.UtcNow
                    };
                    _dbContext.Notifications.Add(notification);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch
            {
                // Non-fatal
            }

            return MapToResponse(interview);
        }

        public async Task<InterviewResponse> SubmitFeedbackAsync(Guid id, SubmitInterviewFeedbackRequest request, Guid userId)
        {
            var interview = await _interviewRepository.GetByIdAsync(id);
            if (interview == null)
                throw new InvalidOperationException("Interview not found");

            interview.Feedback = request.Feedback;
            if (request.Rating.HasValue) interview.Rating = request.Rating.Value;
            interview.Status = string.IsNullOrWhiteSpace(request.Status) ? "COMPLETED" : request.Status;

            await _interviewRepository.UpdateAsync(interview);
            return MapToResponse(interview);
        }

        public async Task<InterviewSessionResponse> CreateSessionAsync(CreateInterviewSessionRequest request, Guid userId)
        {
            var interview = await _interviewRepository.GetByIdAsync(request.InterviewId);
            if (interview == null)
                throw new InvalidOperationException("Interview not found");

            var session = new InterviewSession
            {
                Id = Guid.NewGuid(),
                InterviewId = request.InterviewId,
                SessionNumber = request.SessionNumber,
                Title = request.Title,
                SessionType = request.SessionType,
                ScheduledAt = request.ScheduledAt,
                DurationMinutes = request.DurationMinutes > 0 ? request.DurationMinutes : 60,
                LocationOrLink = request.LocationOrLink,
                Notes = request.Notes,
                InterviewerId = request.InterviewerId ?? interview.InterviewerId,
                Status = "SCHEDULED",
                CreatedDate = DateTime.UtcNow
            };

            await _interviewRepository.AddSessionAsync(session);

            var loaded = await _interviewRepository.GetSessionByIdAsync(session.Id);
            return MapSessionToResponse(loaded ?? session);
        }

        public async Task<InterviewSessionResponse?> GetSessionByIdAsync(Guid sessionId)
        {
            var session = await _interviewRepository.GetSessionByIdAsync(sessionId);
            if (session == null) return null;
            return MapSessionToResponse(session);
        }

        public async Task<List<InterviewSessionResponse>> GetSessionsByInterviewIdAsync(Guid interviewId)
        {
            var sessions = await _interviewRepository.GetSessionsByInterviewIdAsync(interviewId);
            return sessions.Select(MapSessionToResponse).ToList();
        }

        public async Task<InterviewSessionResponse> UpdateSessionAsync(Guid sessionId, UpdateInterviewSessionRequest request, Guid userId)
        {
            var session = await _interviewRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
                throw new InvalidOperationException("Interview session not found");

            if (!string.IsNullOrWhiteSpace(request.Title)) session.Title = request.Title;
            if (!string.IsNullOrWhiteSpace(request.SessionType)) session.SessionType = request.SessionType;
            if (request.ScheduledAt.HasValue) session.ScheduledAt = request.ScheduledAt.Value;
            if (request.DurationMinutes.HasValue) session.DurationMinutes = request.DurationMinutes.Value;
            if (request.LocationOrLink != null) session.LocationOrLink = request.LocationOrLink;
            if (!string.IsNullOrWhiteSpace(request.Status)) session.Status = request.Status;
            if (request.Notes != null) session.Notes = request.Notes;
            if (request.Feedback != null) session.Feedback = request.Feedback;
            if (request.Rating.HasValue) session.Rating = request.Rating.Value;
            if (request.InterviewerId.HasValue) session.InterviewerId = request.InterviewerId.Value;

            await _interviewRepository.UpdateSessionAsync(session);
            return MapSessionToResponse(session);
        }

        public async Task<InterviewSessionResponse> CancelSessionAsync(Guid sessionId, Guid userId, string? reason)
        {
            var session = await _interviewRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
                throw new InvalidOperationException("Interview session not found");

            session.Status = "CANCELLED";
            if (!string.IsNullOrWhiteSpace(reason))
            {
                session.Notes = string.IsNullOrWhiteSpace(session.Notes)
                    ? $"Cancelled: {reason}"
                    : $"{session.Notes} | Cancelled: {reason}";
            }

            await _interviewRepository.UpdateSessionAsync(session);
            return MapSessionToResponse(session);
        }

        public async Task<InterviewQuestionResponse> AddQuestionAsync(CreateInterviewQuestionRequest request, Guid userId)
        {
            var session = await _interviewRepository.GetSessionByIdAsync(request.SessionId);
            if (session == null)
                throw new InvalidOperationException("Interview session not found");

            var question = new InterviewQuestion
            {
                Id = Guid.NewGuid(),
                SessionId = request.SessionId,
                QuestionText = request.QuestionText,
                QuestionType = request.QuestionType,
                Order = request.Order,
                MaxScore = request.MaxScore,
                ExpectedAnswer = request.ExpectedAnswer,
                Category = request.Category,
                Difficulty = request.Difficulty,
                CreatedDate = DateTime.UtcNow
            };

            await _interviewRepository.AddQuestionAsync(question);

            var loaded = await _interviewRepository.GetQuestionByIdAsync(question.Id);
            return MapQuestionToResponse(loaded ?? question);
        }

        public async Task<InterviewQuestionResponse?> GetQuestionByIdAsync(Guid questionId)
        {
            var question = await _interviewRepository.GetQuestionByIdAsync(questionId);
            if (question == null) return null;
            return MapQuestionToResponse(question);
        }

        public async Task<List<InterviewQuestionResponse>> GetQuestionsBySessionIdAsync(Guid sessionId)
        {
            var questions = await _interviewRepository.GetQuestionsBySessionIdAsync(sessionId);
            return questions.Select(MapQuestionToResponse).ToList();
        }

        public async Task<InterviewQuestionResponse> UpdateQuestionAsync(Guid questionId, CreateInterviewQuestionRequest request, Guid userId)
        {
            var question = await _interviewRepository.GetQuestionByIdAsync(questionId);
            if (question == null)
                throw new InvalidOperationException("Interview question not found");

            question.QuestionText = request.QuestionText;
            question.QuestionType = request.QuestionType;
            question.Order = request.Order;
            question.MaxScore = request.MaxScore;
            question.ExpectedAnswer = request.ExpectedAnswer;
            question.Category = request.Category;
            question.Difficulty = request.Difficulty;

            await _interviewRepository.UpdateQuestionAsync(question);
            return MapQuestionToResponse(question);
        }

        public async Task DeleteQuestionAsync(Guid questionId, Guid userId)
        {
            await _interviewRepository.DeleteQuestionAsync(questionId);
        }

        public async Task<InterviewAnswerResponse> SubmitAnswerAsync(SubmitInterviewAnswerRequest request, Guid candidateId)
        {
            var question = await _interviewRepository.GetQuestionByIdAsync(request.QuestionId);
            if (question == null)
                throw new InvalidOperationException("Interview question not found");

            var answer = new InterviewAnswer
            {
                Id = Guid.NewGuid(),
                QuestionId = request.QuestionId,
                CandidateId = candidateId,
                AnswerText = request.AnswerText,
                CreatedDate = DateTime.UtcNow
            };

            await _interviewRepository.AddAnswerAsync(answer);

            var loaded = await _interviewRepository.GetAnswerByIdAsync(answer.Id);
            return MapAnswerToResponse(loaded ?? answer);
        }

        public async Task<List<InterviewAnswerResponse>> GetAnswersByQuestionIdAsync(Guid questionId)
        {
            var answers = await _interviewRepository.GetAnswersByQuestionIdAsync(questionId);
            return answers.Select(MapAnswerToResponse).ToList();
        }

        public async Task<List<InterviewAnswerResponse>> GetAnswersByCandidateIdAsync(Guid candidateId)
        {
            var answers = await _interviewRepository.GetAnswersByCandidateIdAsync(candidateId);
            return answers.Select(MapAnswerToResponse).ToList();
        }

        public async Task<InterviewAnswerResponse> EvaluateAnswerAsync(Guid answerId, int score, string? notes, Guid evaluatorId)
        {
            var answer = await _interviewRepository.GetAnswerByIdAsync(answerId);
            if (answer == null)
                throw new InvalidOperationException("Interview answer not found");

            answer.Score = score;
            answer.EvaluatorNotes = notes;
            answer.EvaluatedById = evaluatorId;
            answer.EvaluatedAt = DateTime.UtcNow;

            await _interviewRepository.UpdateAnswerAsync(answer);
            return MapAnswerToResponse(answer);
        }

        public async Task<InterviewerAvailabilityResponse> CreateAvailabilityAsync(CreateInterviewerAvailabilityRequest request, Guid interviewerId)
        {
            var availability = new InterviewerAvailability
            {
                Id = Guid.NewGuid(),
                InterviewerId = interviewerId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TimeZone = request.TimeZone ?? "UTC",
                SlotDurationMinutes = request.SlotDurationMinutes > 0 ? request.SlotDurationMinutes : 30,
                MaxBookingsPerSlot = request.MaxBookingsPerSlot > 0 ? request.MaxBookingsPerSlot : 1,
                InterviewTypes = request.InterviewTypes,
                JobId = request.JobId,
                CompanyId = request.CompanyId,
                Notes = request.Notes,
                IsRecurring = request.IsRecurring,
                RecurrencePattern = request.RecurrencePattern,
                RecurrenceEndDate = request.RecurrenceEndDate,
                Status = "AVAILABLE",
                CreatedDate = DateTime.UtcNow
            };

            await _interviewRepository.AddAvailabilityAsync(availability);

            var loaded = await _interviewRepository.GetAvailabilityByIdAsync(availability.Id);
            return MapAvailabilityToResponse(loaded ?? availability);
        }

        public async Task<InterviewerAvailabilityResponse?> GetAvailabilityByIdAsync(Guid availabilityId)
        {
            var availability = await _interviewRepository.GetAvailabilityByIdAsync(availabilityId);
            if (availability == null) return null;
            return MapAvailabilityToResponse(availability);
        }

        public async Task<List<InterviewerAvailabilityResponse>> GetMyAvailabilitiesAsync(Guid interviewerId, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var availabilities = await _interviewRepository.GetAvailabilitiesByInterviewerIdAsync(interviewerId, from, to, cancellationToken);
            return availabilities.Select(MapAvailabilityToResponse).ToList();
        }

        public async Task<List<InterviewerAvailabilityResponse>> SearchAvailabilitiesAsync(Guid? companyId, Guid? interviewerId, Guid? jobId, DateTime? from, DateTime? to, string? status, int limit, int offset, CancellationToken cancellationToken)
        {
            var availabilities = await _interviewRepository.SearchAvailabilitiesAsync(companyId, interviewerId, jobId, from, to, status, limit, offset, cancellationToken);
            return availabilities.Select(MapAvailabilityToResponse).ToList();
        }

        public async Task<InterviewerAvailabilityResponse> UpdateAvailabilityAsync(Guid availabilityId, UpdateInterviewerAvailabilityRequest request, Guid userId)
        {
            var availability = await _interviewRepository.GetAvailabilityByIdAsync(availabilityId);
            if (availability == null)
                throw new InvalidOperationException("Interviewer availability not found");

            if (request.StartTime.HasValue) availability.StartTime = request.StartTime.Value;
            if (request.EndTime.HasValue) availability.EndTime = request.EndTime.Value;
            if (request.TimeZone != null) availability.TimeZone = request.TimeZone;
            if (request.SlotDurationMinutes.HasValue) availability.SlotDurationMinutes = request.SlotDurationMinutes.Value;
            if (request.MaxBookingsPerSlot.HasValue) availability.MaxBookingsPerSlot = request.MaxBookingsPerSlot.Value;
            if (request.InterviewTypes != null) availability.InterviewTypes = request.InterviewTypes;
            if (request.Status != null) availability.Status = request.Status;
            if (request.Notes != null) availability.Notes = request.Notes;
            if (request.IsRecurring.HasValue) availability.IsRecurring = request.IsRecurring.Value;
            if (request.RecurrencePattern != null) availability.RecurrencePattern = request.RecurrencePattern;
            if (request.RecurrenceEndDate.HasValue) availability.RecurrenceEndDate = request.RecurrenceEndDate.Value;

            await _interviewRepository.UpdateAvailabilityAsync(availability);
            return MapAvailabilityToResponse(availability);
        }

        public async Task DeleteAvailabilityAsync(Guid availabilityId, Guid userId)
        {
            await _interviewRepository.DeleteAvailabilityAsync(availabilityId);
        }

        public async Task<InterviewBookingResponse> BookAsync(CreateInterviewBookingRequest request, Guid candidateId)
        {
            var availability = await _interviewRepository.GetAvailabilityByIdAsync(request.AvailabilityId);
            if (availability == null)
                throw new InvalidOperationException("Availability slot not found");

            if (availability.Status != "AVAILABLE")
                throw new InvalidOperationException("Availability slot is not available for booking");

            var existingBookings = await _interviewRepository.GetBookingsByAvailabilityIdAsync(request.AvailabilityId);
            var activeBookings = existingBookings.Where(b => b.Status != "CANCELLED_BY_CANDIDATE" && b.Status != "CANCELLED_BY_INTERVIEWER").Count();

            if (activeBookings >= availability.MaxBookingsPerSlot)
                throw new InvalidOperationException("Availability slot is fully booked");

            var hasConflict = await _interviewRepository.HasConflictingBookingAsync(
                availability.InterviewerId,
                availability.StartTime,
                availability.SlotDurationMinutes,
                null,
                CancellationToken.None);

            if (hasConflict)
                throw new InvalidOperationException("Interviewer has a conflicting booking at this time");

            var candidateProfile = await _dbContext.CandidateProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == candidateId);

            if (candidateProfile == null)
                throw new InvalidOperationException("Candidate profile not found");

            var booking = new InterviewBooking
            {
                Id = Guid.NewGuid(),
                AvailabilityId = request.AvailabilityId,
                CandidateId = candidateId,
                JobId = request.JobId ?? availability.JobId,
                ApplicationId = request.ApplicationId,
                ScheduledAt = availability.StartTime,
                DurationMinutes = availability.SlotDurationMinutes,
                Status = "BOOKED",
                Notes = request.Notes,
                LocationOrLink = availability.Job?.Title != null ? "TBD" : null,
                CreatedDate = DateTime.UtcNow
            };

            await _interviewRepository.AddBookingAsync(booking);

            var activeCount = existingBookings.Where(b => b.Status != "CANCELLED_BY_CANDIDATE" && b.Status != "CANCELLED_BY_INTERVIEWER").Count() + 1;
            if (activeCount >= availability.MaxBookingsPerSlot)
            {
                availability.Status = "FULLY_BOOKED";
                await _interviewRepository.UpdateAvailabilityAsync(availability);
            }

            var loaded = await _interviewRepository.GetBookingByIdAsync(booking.Id);
            return MapBookingToResponse(loaded ?? booking);
        }

        public async Task<InterviewBookingResponse?> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _interviewRepository.GetBookingByIdAsync(bookingId);
            if (booking == null) return null;
            return MapBookingToResponse(booking);
        }

        public async Task<List<InterviewBookingResponse>> GetMyBookingsAsync(Guid candidateId, string? status, CancellationToken cancellationToken)
        {
            var bookings = await _interviewRepository.GetBookingsByCandidateIdAsync(candidateId, status, cancellationToken);
            return bookings.Select(MapBookingToResponse).ToList();
        }

        public async Task<List<InterviewBookingResponse>> GetBookingsForInterviewerAsync(Guid interviewerId, string? status, CancellationToken cancellationToken)
        {
            var bookings = await _interviewRepository.GetBookingsByInterviewerIdAsync(interviewerId, status, cancellationToken);
            return bookings.Select(MapBookingToResponse).ToList();
        }

        public async Task<InterviewBookingResponse> UpdateBookingAsync(Guid bookingId, UpdateInterviewBookingRequest request, Guid userId)
        {
            var booking = await _interviewRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
                throw new InvalidOperationException("Interview booking not found");

            if (!string.IsNullOrWhiteSpace(request.Status)) booking.Status = request.Status;
            if (request.Notes != null) booking.Notes = request.Notes;
            if (request.LocationOrLink != null) booking.LocationOrLink = request.LocationOrLink;

            await _interviewRepository.UpdateBookingAsync(booking);
            return MapBookingToResponse(booking);
        }

        public async Task<InterviewBookingResponse> CancelBookingAsync(Guid bookingId, Guid userId, string? reason)
        {
            var booking = await _interviewRepository.GetBookingByIdAsync(bookingId);
            if (booking == null)
                throw new InvalidOperationException("Interview booking not found");

            var isCandidate = booking.CandidateId == userId;
            booking.Status = isCandidate ? "CANCELLED_BY_CANDIDATE" : "CANCELLED_BY_INTERVIEWER";
            booking.CancellationReason = reason;
            booking.CancelledById = userId;
            booking.CancelledAt = DateTime.UtcNow;

            await _interviewRepository.UpdateBookingAsync(booking);

            var availability = await _interviewRepository.GetAvailabilityByIdAsync(booking.AvailabilityId);
            if (availability != null)
            {
                var existingBookings = await _interviewRepository.GetBookingsByAvailabilityIdAsync(availability.Id);
                var activeBookings = existingBookings.Where(b => b.Status != "CANCELLED_BY_CANDIDATE" && b.Status != "CANCELLED_BY_INTERVIEWER").Count();

                if (activeBookings < availability.MaxBookingsPerSlot && availability.Status == "FULLY_BOOKED")
                {
                    availability.Status = "AVAILABLE";
                    await _interviewRepository.UpdateAvailabilityAsync(availability);
                }
            }

            return MapBookingToResponse(booking);
        }

        public async Task<InterviewEvaluationResponse> CreateEvaluationAsync(CreateInterviewEvaluationRequest request, Guid evaluatorId)
        {
            var session = await _interviewRepository.GetSessionByIdAsync(request.SessionId);
            if (session == null)
                throw new InvalidOperationException("Interview session not found");

            var interview = await _interviewRepository.GetByIdAsync(request.InterviewId);
            if (interview == null)
                throw new InvalidOperationException("Interview not found");

            var evaluation = new InterviewEvaluation
            {
                Id = Guid.NewGuid(),
                SessionId = request.SessionId,
                InterviewId = request.InterviewId,
                EvaluatorId = evaluatorId,
                CandidateId = request.CandidateId,
                OverallScore = request.OverallScore,
                OverallRating = request.OverallRating,
                Summary = request.Summary,
                Strengths = request.Strengths,
                Weaknesses = request.Weaknesses,
                Recommendation = request.Recommendation,
                IsSubmitted = false,
                CreatedDate = DateTime.UtcNow
            };

            await _interviewRepository.AddEvaluationAsync(evaluation);

            foreach (var criteria in request.CriteriaScores)
            {
                var criteriaEntity = new InterviewEvaluationCriteria
                {
                    Id = Guid.NewGuid(),
                    EvaluationId = evaluation.Id,
                    CriteriaName = criteria.CriteriaName,
                    CriteriaCategory = criteria.CriteriaCategory,
                    Score = criteria.Score,
                    MaxScore = criteria.MaxScore > 0 ? criteria.MaxScore : 5,
                    Notes = criteria.Notes,
                    Weight = criteria.Weight > 0 ? criteria.Weight : 1,
                    CreatedDate = DateTime.UtcNow
                };
                await _interviewRepository.AddEvaluationCriteriaAsync(criteriaEntity);
            }

            var loaded = await _interviewRepository.GetEvaluationByIdAsync(evaluation.Id);
            return MapEvaluationToResponse(loaded ?? evaluation);
        }

        public async Task<InterviewEvaluationResponse?> GetEvaluationByIdAsync(Guid evaluationId)
        {
            var evaluation = await _interviewRepository.GetEvaluationByIdAsync(evaluationId);
            if (evaluation == null) return null;
            return MapEvaluationToResponse(evaluation);
        }

        public async Task<List<InterviewEvaluationResponse>> GetEvaluationsByInterviewIdAsync(Guid interviewId)
        {
            var evaluations = await _interviewRepository.GetEvaluationsByInterviewIdAsync(interviewId);
            return evaluations.Select(MapEvaluationToResponse).ToList();
        }

        public async Task<List<InterviewEvaluationResponse>> GetEvaluationsBySessionIdAsync(Guid sessionId)
        {
            var evaluations = await _interviewRepository.GetEvaluationsBySessionIdAsync(sessionId);
            return evaluations.Select(MapEvaluationToResponse).ToList();
        }

        public async Task<InterviewEvaluationResponse> SubmitEvaluationAsync(Guid evaluationId, Guid evaluatorId)
        {
            var evaluation = await _interviewRepository.GetEvaluationByIdAsync(evaluationId);
            if (evaluation == null)
                throw new InvalidOperationException("Interview evaluation not found");

            if (evaluation.EvaluatorId != evaluatorId)
                throw new InvalidOperationException("Only the assigned evaluator can submit this evaluation");

            evaluation.IsSubmitted = true;
            evaluation.SubmittedAt = DateTime.UtcNow;

            await _interviewRepository.UpdateEvaluationAsync(evaluation);
            return MapEvaluationToResponse(evaluation);
        }

        public async Task<InterviewReportResponse> GenerateReportAsync(CreateInterviewReportRequest request, Guid generatedById)
        {
            var interview = await _interviewRepository.GetByIdAsync(request.InterviewId);
            if (interview == null)
                throw new InvalidOperationException("Interview not found");

            var report = new InterviewReport
            {
                Id = Guid.NewGuid(),
                InterviewId = request.InterviewId,
                CandidateId = interview.CandidateId,
                GeneratedById = generatedById,
                ReportType = request.ReportType,
                Title = request.Title,
                Content = request.Content,
                ExecutiveSummary = request.ExecutiveSummary,
                FinalRecommendation = request.FinalRecommendation,
                OverallScore = request.OverallScore,
                Status = "DRAFT",
                CreatedDate = DateTime.UtcNow
            };

            await _interviewRepository.AddReportAsync(report);

            var loaded = await _interviewRepository.GetReportByIdAsync(report.Id);
            return MapReportToResponse(loaded ?? report);
        }

        public async Task<InterviewReportResponse?> GetReportByIdAsync(Guid reportId)
        {
            var report = await _interviewRepository.GetReportByIdAsync(reportId);
            if (report == null) return null;
            return MapReportToResponse(report);
        }

        public async Task<List<InterviewReportResponse>> GetReportsByInterviewIdAsync(Guid interviewId)
        {
            var reports = await _interviewRepository.GetReportsByInterviewIdAsync(interviewId);
            return reports.Select(MapReportToResponse).ToList();
        }

        public async Task<InterviewReportResponse> FinalizeReportAsync(Guid reportId, Guid userId)
        {
            var report = await _interviewRepository.GetReportByIdAsync(reportId);
            if (report == null)
                throw new InvalidOperationException("Interview report not found");

            if (report.GeneratedById != userId)
                throw new InvalidOperationException("Only the report author can finalize this report");

            report.Status = "FINALIZED";
            report.FinalizedAt = DateTime.UtcNow;

            await _interviewRepository.UpdateReportAsync(report);
            return MapReportToResponse(report);
        }

        private static InterviewResponse MapToResponse(Interview i)
        {
            var candidateUser = i.Candidate?.User;
            var candidateName = candidateUser != null ? $"{candidateUser.FirstName} {candidateUser.LastName}".Trim() : null;
            var interviewerName = i.Interviewer != null ? $"{i.Interviewer.FirstName} {i.Interviewer.LastName}".Trim() : null;

            return new InterviewResponse
            {
                Id = i.Id,
                ApplicationId = i.ApplicationId,
                CandidateId = i.CandidateId,
                CandidateName = string.IsNullOrEmpty(candidateName) ? null : candidateName,
                CandidateEmail = candidateUser?.Email,
                JobId = i.JobId,
                JobTitle = i.Job?.Title,
                CompanyId = i.CompanyId,
                CompanyName = i.Company?.Name,
                InterviewerId = i.InterviewerId,
                InterviewerName = string.IsNullOrEmpty(interviewerName) ? null : interviewerName,
                Title = i.Title,
                InterviewType = i.InterviewType,
                ScheduledAt = i.ScheduledAt,
                DurationMinutes = i.DurationMinutes,
                LocationOrLink = i.LocationOrLink,
                Status = i.Status,
                Notes = i.Notes,
                Feedback = i.Feedback,
                Rating = i.Rating,
                CreatedDate = i.CreatedDate
            };
        }

        private static InterviewSessionResponse MapSessionToResponse(InterviewSession s)
        {
            var interviewerName = s.Interviewer != null ? $"{s.Interviewer.FirstName} {s.Interviewer.LastName}".Trim() : null;

            return new InterviewSessionResponse
            {
                Id = s.Id,
                InterviewId = s.InterviewId,
                SessionNumber = s.SessionNumber,
                Title = s.Title,
                SessionType = s.SessionType,
                ScheduledAt = s.ScheduledAt,
                DurationMinutes = s.DurationMinutes,
                LocationOrLink = s.LocationOrLink,
                Status = s.Status,
                Notes = s.Notes,
                Feedback = s.Feedback,
                Rating = s.Rating,
                InterviewerId = s.InterviewerId,
                InterviewerName = string.IsNullOrEmpty(interviewerName) ? null : interviewerName,
                CreatedDate = s.CreatedDate
            };
        }

        private static InterviewQuestionResponse MapQuestionToResponse(InterviewQuestion q)
        {
            return new InterviewQuestionResponse
            {
                Id = q.Id,
                SessionId = q.SessionId,
                QuestionText = q.QuestionText,
                QuestionType = q.QuestionType,
                Order = q.Order,
                MaxScore = q.MaxScore,
                ExpectedAnswer = q.ExpectedAnswer,
                Category = q.Category,
                Difficulty = q.Difficulty,
                CreatedDate = q.CreatedDate
            };
        }

        private static InterviewAnswerResponse MapAnswerToResponse(InterviewAnswer a)
        {
            var candidateName = a.Candidate != null ? $"{a.Candidate.FirstName} {a.Candidate.LastName}".Trim() : string.Empty;
            var evaluatedByName = a.EvaluatedBy != null ? $"{a.EvaluatedBy.FirstName} {a.EvaluatedBy.LastName}".Trim() : null;

            return new InterviewAnswerResponse
            {
                Id = a.Id,
                QuestionId = a.QuestionId,
                QuestionText = a.Question?.QuestionText ?? string.Empty,
                CandidateId = a.CandidateId,
                CandidateName = candidateName,
                AnswerText = a.AnswerText,
                Score = a.Score,
                EvaluatorNotes = a.EvaluatorNotes,
                EvaluatedById = a.EvaluatedById,
                EvaluatedByName = string.IsNullOrEmpty(evaluatedByName) ? null : evaluatedByName,
                EvaluatedAt = a.EvaluatedAt,
                CreatedDate = a.CreatedDate
            };
        }

        private static InterviewerAvailabilityResponse MapAvailabilityToResponse(InterviewerAvailability a)
        {
            var interviewerName = a.Interviewer != null ? $"{a.Interviewer.FirstName} {a.Interviewer.LastName}".Trim() : string.Empty;
            var currentBookings = a.Bookings?.Where(b => !b.IsDeleted && b.Status != "CANCELLED_BY_CANDIDATE" && b.Status != "CANCELLED_BY_INTERVIEWER").Count() ?? 0;

            return new InterviewerAvailabilityResponse
            {
                Id = a.Id,
                InterviewerId = a.InterviewerId,
                InterviewerName = interviewerName,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                TimeZone = a.TimeZone ?? "UTC",
                SlotDurationMinutes = a.SlotDurationMinutes,
                MaxBookingsPerSlot = a.MaxBookingsPerSlot,
                CurrentBookingsCount = currentBookings,
                InterviewTypes = a.InterviewTypes,
                JobId = a.JobId,
                JobTitle = a.Job?.Title,
                CompanyId = a.CompanyId,
                Status = a.Status,
                Notes = a.Notes,
                IsRecurring = a.IsRecurring,
                RecurrencePattern = a.RecurrencePattern,
                RecurrenceEndDate = a.RecurrenceEndDate,
                CreatedDate = a.CreatedDate
            };
        }

        private static InterviewBookingResponse MapBookingToResponse(InterviewBooking b)
        {
            var candidateName = b.Candidate != null ? $"{b.Candidate.FirstName} {b.Candidate.LastName}".Trim() : string.Empty;
            var cancelledByName = b.CancelledBy != null ? $"{b.CancelledBy.FirstName} {b.CancelledBy.LastName}".Trim() : null;

            return new InterviewBookingResponse
            {
                Id = b.Id,
                AvailabilityId = b.AvailabilityId,
                CandidateId = b.CandidateId,
                CandidateName = candidateName,
                CandidateEmail = b.Candidate?.Email ?? string.Empty,
                JobId = b.JobId,
                JobTitle = b.Job?.Title,
                ApplicationId = b.ApplicationId,
                BookedAt = b.BookedAt,
                ScheduledAt = b.ScheduledAt,
                DurationMinutes = b.DurationMinutes,
                Status = b.Status,
                CancellationReason = b.CancellationReason,
                CancelledById = b.CancelledById,
                CancelledByName = string.IsNullOrEmpty(cancelledByName) ? null : cancelledByName,
                CancelledAt = b.CancelledAt,
                LocationOrLink = b.LocationOrLink,
                Notes = b.Notes,
                CreatedDate = b.CreatedDate
            };
        }

        private static InterviewEvaluationResponse MapEvaluationToResponse(InterviewEvaluation e)
        {
            var evaluatorName = e.Evaluator != null ? $"{e.Evaluator.FirstName} {e.Evaluator.LastName}".Trim() : string.Empty;
            var candidateName = e.Candidate != null ? $"{e.Candidate.FirstName} {e.Candidate.LastName}".Trim() : string.Empty;

            return new InterviewEvaluationResponse
            {
                Id = e.Id,
                SessionId = e.SessionId,
                InterviewId = e.InterviewId,
                EvaluatorId = e.EvaluatorId,
                EvaluatorName = evaluatorName,
                CandidateId = e.CandidateId,
                CandidateName = candidateName,
                OverallScore = e.OverallScore,
                OverallRating = e.OverallRating,
                Summary = e.Summary,
                Strengths = e.Strengths,
                Weaknesses = e.Weaknesses,
                Recommendation = e.Recommendation,
                IsSubmitted = e.IsSubmitted,
                SubmittedAt = e.SubmittedAt,
                CreatedDate = e.CreatedDate,
                CriteriaScores = e.CriteriaScores?.Select(c => new InterviewEvaluationCriteriaResponse
                {
                    Id = c.Id,
                    CriteriaName = c.CriteriaName,
                    CriteriaCategory = c.CriteriaCategory,
                    Score = c.Score,
                    MaxScore = c.MaxScore,
                    Notes = c.Notes,
                    Weight = c.Weight
                }).ToList() ?? new List<InterviewEvaluationCriteriaResponse>()
            };
        }

        private static InterviewReportResponse MapReportToResponse(InterviewReport r)
        {
            var candidateName = r.Candidate != null ? $"{r.Candidate.FirstName} {r.Candidate.LastName}".Trim() : string.Empty;
            var generatedByName = r.GeneratedBy != null ? $"{r.GeneratedBy.FirstName} {r.GeneratedBy.LastName}".Trim() : string.Empty;

            return new InterviewReportResponse
            {
                Id = r.Id,
                InterviewId = r.InterviewId,
                CandidateId = r.CandidateId,
                CandidateName = candidateName,
                GeneratedById = r.GeneratedById,
                GeneratedByName = generatedByName,
                ReportType = r.ReportType,
                Title = r.Title,
                Content = r.Content,
                ExecutiveSummary = r.ExecutiveSummary,
                FinalRecommendation = r.FinalRecommendation,
                OverallScore = r.OverallScore,
                Status = r.Status,
                FinalizedAt = r.FinalizedAt,
                CreatedDate = r.CreatedDate
            };
        }
    }
}

