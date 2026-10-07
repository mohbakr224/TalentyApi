using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities.Applications;
using Domain.Entities.Notifications;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Services.Applications
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly AppDBcontext _dbContext;

        private static readonly Dictionary<string, List<string>> _allowedTransitions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "APPLIED", new List<string>{ "SCREENING", "SHORTLISTED", "INTERVIEW", "WITHDRAWN", "REJECTED" } },
            { "SCREENING", new List<string>{ "SHORTLISTED", "INTERVIEW", "WITHDRAWN", "REJECTED" } },
            { "SHORTLISTED", new List<string>{ "SCREENING", "INTERVIEW", "OFFER", "WITHDRAWN", "REJECTED" } },
            { "INTERVIEW", new List<string>{ "SHORTLISTED", "OFFER", "WITHDRAWN", "REJECTED" } },
            { "OFFER", new List<string>{ "HIRED", "WITHDRAWN", "REJECTED" } },
            { "HIRED", new List<string>{ "WITHDRAWN" } },
            { "REJECTED", new List<string>{ "SCREENING", "SHORTLISTED" } },
            { "WITHDRAWN", new List<string>() }
        };

        public ApplicationService(IApplicationRepository applicationRepository, AppDBcontext dbContext)
        {
            _applicationRepository = applicationRepository;
            _dbContext = dbContext;
        }

        public async Task<ApplicationResponse> CreateAsync(CreateApplicationRequest request, Guid candidateId)
        {
            var candidate = await _dbContext.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == candidateId && !p.IsDeleted);

            if (candidate == null)
                throw new InvalidOperationException("Candidate profile not found");

            var selectedResumeId = request.ResumeId;
            if (selectedResumeId.HasValue)
            {
                var selectedResume = await _dbContext.Resumes
                    .FirstOrDefaultAsync(r => r.Id == selectedResumeId.Value
                        && r.CandidateId == candidate.Id
                        && !r.IsDeleted);

                if (selectedResume == null)
                    throw new InvalidOperationException("Resume not found or does not belong to the candidate");

                if (!selectedResume.CurrentVersionId.HasValue)
                    throw new InvalidOperationException("The selected resume has no current version");
            }
            else
            {
                var currentResume = await _dbContext.Resumes
                    .Where(r => r.CandidateId == candidate.Id
                        && !r.IsDeleted
                        && r.CurrentVersionId.HasValue)
                    .OrderByDescending(r => r.UpdatedAt ?? r.CreatedDate)
                    .FirstOrDefaultAsync();

                if (currentResume == null)
                    throw new InvalidOperationException("No active resume is available for this application");

                selectedResumeId = currentResume.Id;
            }

            // Prevent duplicate active application for same candidate and job
            var existing = await _applicationRepository.GetByCandidateIdAsync(candidate.Id, CancellationToken.None);
            if (existing != null && existing.Any(a => a.JobId == request.JobId && (a.Status == null || (a.Status != "WITHDRAWN" && a.Status != "REJECTED"))))
                throw new InvalidOperationException("You have already applied to this job");

            var entity = new Domain.Entities.Applications.Application
            {
                Id = Guid.NewGuid(),
                CandidateId = candidate.Id,
                JobId = request.JobId,
                ResumeId = selectedResumeId,
                CoverLetter = request.CoverLetter,
                Status = "APPLIED",
                CreatedDate = DateTime.UtcNow
            };

            await _applicationRepository.AddAsync(entity);

            // Fetch loaded entity with navigation details
            var loaded = await _applicationRepository.GetByIdAsync(entity.Id);
            return MapToResponse(loaded ?? entity);
        }

        public async Task<ApplicationResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _applicationRepository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToResponse(entity);
        }

        public async Task<List<ApplicationResponse>> GetByCandidateIdAsync(Guid candidateId, CancellationToken cancellationToken)
        {
            var list = await _applicationRepository.GetByCandidateIdAsync(candidateId, cancellationToken);
            return list.Select(MapToResponse).ToList();
        }

        public async Task<List<ApplicationResponse>> GetByJobIdAsync(Guid jobId, string? status, int limit, int offset, CancellationToken cancellationToken)
        {
            var list = await _applicationRepository.GetByJobIdAsync(jobId, status, limit, offset, cancellationToken);
            return list.Select(MapToResponse).ToList();
        }

        public async Task<List<ApplicationResponse>> GetByCompanyIdAsync(Guid companyId, string? status, int limit, int offset, CancellationToken cancellationToken)
        {
            var list = await _applicationRepository.GetByCompanyIdAsync(companyId, status, limit, offset, cancellationToken);
            return list.Select(MapToResponse).ToList();
        }

        public async Task<ApplicationResponse> ShortlistAsync(Guid applicationId, Guid changedBy, string? reason = null)
        {
            return await ChangeStatusAsync(applicationId, "SHORTLISTED", changedBy, reason ?? "Candidate shortlisted by HR");
        }

        public async Task<ApplicationResponse> RejectAsync(Guid applicationId, Guid changedBy, string? reason = null)
        {
            return await ChangeStatusAsync(applicationId, "REJECTED", changedBy, reason ?? "Application rejected by HR");
        }

        public async Task<ApplicationResponse> ChangeStatusAsync(Guid applicationId, string newStatus, Guid changedBy, string? reason = null)
        {
            var app = await _applicationRepository.GetByIdAsync(applicationId);
            if (app == null)
                throw new InvalidOperationException("Application not found");

            var current = app.Status ?? "APPLIED";
            if (!_allowedTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(newStatus, StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Invalid status transition from {current} to {newStatus}");

            var history = new ApplicationStatusHistory
            {
                Id = Guid.NewGuid(),
                ApplicationId = app.Id,
                FromStatus = current,
                ToStatus = newStatus,
                ChangedBy = changedBy,
                Reason = reason,
                CreatedDate = DateTime.UtcNow
            };

            app.Status = newStatus;
            app.StatusHistory.Add(history);
            app.UpdatedAt = DateTime.UtcNow;

            await _applicationRepository.UpdateAsync(app);

            // Send notification to candidate user
            try
            {
                var candidateUser = app.Candidate?.User ?? (app.Candidate != null ? await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == app.Candidate.UserId) : null);
                if (candidateUser != null)
                {
                    var jobTitle = app.Job?.Title ?? "the position";
                    var companyName = app.Job?.Company?.Name ?? "the company";
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = candidateUser.Id,
                        SenderId = changedBy,
                        CompanyId = app.Job?.CompanyId,
                        Title = $"Application Status Update: {newStatus}",
                        Message = $"Your application for '{jobTitle}' at {companyName} has been changed to '{newStatus}'.{(string.IsNullOrWhiteSpace(reason) ? "" : $" Reason: {reason}")}",
                        Type = "APPLICATION_STATUS",
                        RelatedEntityId = app.Id,
                        IsRead = false,
                        CreatedDate = DateTime.UtcNow
                    };
                    _dbContext.Notifications.Add(notification);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch
            {
                // Notification failure should not fail status update
            }

            return MapToResponse(app);
        }

        private static ApplicationResponse MapToResponse(Domain.Entities.Applications.Application a)
        {
            var candidateUser = a.Candidate?.User;
            var fullName = candidateUser != null ? $"{candidateUser.FirstName} {candidateUser.LastName}".Trim() : null;

            return new ApplicationResponse
            {
                Id = a.Id,
                CandidateId = a.CandidateId,
                JobId = a.JobId,
                ResumeId = a.ResumeId,
                ResumeName = a.Resume?.Name,
                Status = a.Status,
                CoverLetter = a.CoverLetter,
                CandidateName = string.IsNullOrEmpty(fullName) ? null : fullName,
                CandidateEmail = candidateUser?.Email,
                CandidateHeadline = a.Candidate?.Headline,
                JobTitle = a.Job?.Title,
                CompanyId = a.Job?.CompanyId,
                CompanyName = a.Job?.Company?.Name,
                CreatedDate = a.CreatedDate,
                StatusHistory = a.StatusHistory?.Select(h => new ApplicationStatusHistoryResponse
                {
                    Id = h.Id,
                    FromStatus = h.FromStatus,
                    ToStatus = h.ToStatus,
                    ChangedBy = h.ChangedBy,
                    Reason = h.Reason,
                    CreatedDate = h.CreatedDate
                }).OrderBy(h => h.CreatedDate).ToList() ?? new List<ApplicationStatusHistoryResponse>()
            };
        }
    }
}
