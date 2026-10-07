using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Services;
using Domain.Entities.Jobs;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobEntity = Domain.Entities.Jobs.Job;

namespace infrastructure.Services.Job
{
    public class JobService : IJobService
    {
        private readonly AppDBcontext _dbContext;

        public JobService(AppDBcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<JobResponse> CreateAsync(CreateJobRequest request)
        {
            var isPublished = string.Equals(request.Status, "PUBLISHED", StringComparison.OrdinalIgnoreCase);

            var job = new JobEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                CompanyId = request.CompanyId,
                Industry = request.Industry,
                Location = request.Location,
                WorkType = request.WorkType,
                ExperienceLevel = request.ExperienceLevel,
                SalaryMin = request.SalaryMin,
                SalaryMax = request.SalaryMax,
                Currency = request.Currency,
                Status = isPublished ? "PUBLISHED" : (request.Status ?? "DRAFT"),
                PublishedAt = isPublished ? DateTime.UtcNow : null,
                ClosedAt = request.ClosedAt,
                CreatedDate = DateTime.UtcNow
            };

            if (request.SkillIds != null && request.SkillIds.Any())
            {
                foreach (var skillId in request.SkillIds)
                {
                    job.Skills.Add(new JobSkill
                    {
                        JobId = job.Id,
                        SkillId = skillId
                    });
                }
            }

            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync();

            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == job.CompanyId);
            return MapToResponse(job, company?.Name);
        }

        public async Task<JobResponse?> GetByIdAsync(Guid id)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Company)
                .Include(j => j.Skills)
                .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);

            if (job == null) return null;
            return MapToResponse(job, job.Company?.Name);
        }

        public async Task<JobResponse> UpdateAsync(Guid id, UpdateJobRequest request, Guid userId)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Company)
                .Include(j => j.Skills)
                .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);

            if (job == null)
                throw new InvalidOperationException("Job not found");

            if (request.Title != null) job.Title = request.Title;
            if (request.Description != null) job.Description = request.Description;
            if (request.Industry != null) job.Industry = request.Industry;
            if (request.Location != null) job.Location = request.Location;
            if (request.WorkType != null) job.WorkType = request.WorkType;
            if (request.ExperienceLevel != null) job.ExperienceLevel = request.ExperienceLevel;
            if (request.SalaryMin.HasValue) job.SalaryMin = request.SalaryMin;
            if (request.SalaryMax.HasValue) job.SalaryMax = request.SalaryMax;
            if (request.Currency != null) job.Currency = request.Currency;
            if (request.ClosedAt.HasValue) job.ClosedAt = request.ClosedAt;

            if (request.SkillIds != null)
            {
                _dbContext.JobSkills.RemoveRange(job.Skills);
                job.Skills.Clear();
                foreach (var sId in request.SkillIds)
                {
                    job.Skills.Add(new JobSkill
                    {
                        JobId = job.Id,
                        SkillId = sId
                    });
                }
            }

            job.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(job, job.Company?.Name);
        }

        public async Task<JobResponse> PublishAsync(Guid id, Guid userId)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);

            if (job == null)
                throw new InvalidOperationException("Job not found");

            job.Status = "PUBLISHED";
            job.PublishedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(job, job.Company?.Name);
        }

        public async Task<JobResponse> CloseAsync(Guid id, Guid userId)
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == id && !j.IsDeleted);

            if (job == null)
                throw new InvalidOperationException("Job not found");

            job.Status = "CLOSED";
            job.ClosedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return MapToResponse(job, job.Company?.Name);
        }

        public async Task<List<JobResponse>> SearchAsync(string? title, int limit, int offset, CancellationToken cancellationToken)
        {
            var q = _dbContext.Jobs.AsQueryable().Where(j => !j.IsDeleted);
            if (!string.IsNullOrEmpty(title))
                q = q.Where(j => EF.Functions.ILike(j.Title, $"%{title}%"));

            var list = await q.OrderByDescending(j => j.CreatedDate).Skip(offset).Take(limit).ToListAsync(cancellationToken);
            var companyIds = list.Select(l => l.CompanyId).Distinct().ToList();
            var companies = await _dbContext.Companies.Where(c => companyIds.Contains(c.Id)).ToListAsync(cancellationToken);

            return list.Select(j => MapToResponse(j, companies.FirstOrDefault(c => c.Id == j.CompanyId)?.Name)).ToList();
        }

        public async Task<List<JobResponse>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId, cancellationToken);
            var list = await _dbContext.Jobs
                .Where(j => j.CompanyId == companyId && !j.IsDeleted)
                .OrderByDescending(j => j.CreatedDate)
                .ToListAsync(cancellationToken);

            return list.Select(j => MapToResponse(j, company?.Name)).ToList();
        }

        private static JobResponse MapToResponse(JobEntity job, string? companyName)
        {
            return new JobResponse
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                CompanyId = job.CompanyId,
                CompanyName = companyName,
                Location = job.Location,
                WorkType = job.WorkType,
                ExperienceLevel = job.ExperienceLevel,
                Industry = job.Industry,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                Currency = job.Currency,
                Status = job.Status,
                PublishedAt = job.PublishedAt,
                ClosedAt = job.ClosedAt,
                CreatedBy = job.CreatedBy,
                CreatedDate = job.CreatedDate
            };
        }
    }
}
