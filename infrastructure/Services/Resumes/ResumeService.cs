using Application.DTOs.Response;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Domain.Entities.Resumes;
using infrastructure.Persistence;
using infrastructure.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Services.Resumes
{
    public class ResumeService : IResumeService
    {
        private readonly IResumeRepository _resumeRepository;
        private readonly IFileService _fileService;
        private readonly AppDBcontext _dbContext;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ResumeService> _logger;

        // Allowed mime types and extensions
        private static readonly Dictionary<string, string[]> _allowed = new()
        {
            { "application/pdf", new[] { ".pdf" } }
        };

        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public ResumeService(IResumeRepository resumeRepository, IFileService fileService, AppDBcontext dbContext, IBackgroundTaskQueue taskQueue, IServiceScopeFactory scopeFactory, ILogger<ResumeService> logger)
        {
            _resumeRepository = resumeRepository;
            _fileService = fileService;
            _dbContext = dbContext;
            _taskQueue = taskQueue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        private void ValidateFile(string fileName, string contentType, long size)
        {
            if (size <= 0) throw new InvalidOperationException("File is empty");
            if (size > MaxFileSize) throw new InvalidOperationException("File is too large");

            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(ext)) throw new InvalidOperationException("Invalid file extension");

            if (!_allowed.TryGetValue(contentType, out var exts) || !exts.Contains(ext))
                throw new InvalidOperationException("Unsupported file type");
        }

        public async Task<ResumeResponse> UploadAsync(Stream fileStream, string fileName, string contentType, string? name, Guid userId, CancellationToken cancellationToken)
        {
            // Resolve candidate profile id by userId
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");

            // Save to file storage
            var upload = await _fileService.UploadAsync(fileStream, fileName, contentType, cancellationToken);

            // Validate file using size and content type
            ValidateFile(upload.FileName, upload.ContentType, upload.Size);

            // Transactional create resume and version
            var resume = new Resume
            {
                Id = Guid.NewGuid(),
                CandidateId = profile.Id,
                FileId = upload.FileId,
                Name = name ?? upload.FileName,
                Status = "PROCESSING",
                CreatedDate = DateTime.UtcNow
            };

            var version = new ResumeVersion
            {
                Id = Guid.NewGuid(),
                ResumeId = resume.Id,
                VersionNumber = 1,
                CreatedDate = DateTime.UtcNow,
                ExtractedText = null,
                StructuredData = null
            };

            using var tx = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _dbContext.Resumes.AddAsync(resume, cancellationToken);
                await _dbContext.ResumeVersions.AddAsync(version, cancellationToken);
                resume.CurrentVersionId = version.Id;
                await _dbContext.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }

            // Enqueue background parsing job
            _taskQueue.Enqueue(ct => ProcessResumeAsync(upload.FileId, resume.Id, version.Id, ct));

            return new ResumeResponse
            {
                Id = resume.Id,
                Name = resume.Name,
                Status = resume.Status,
                CurrentVersionId = resume.CurrentVersionId,
                CurrentVersionNumber = 1,
                CreatedDate = resume.CreatedDate
            };
        }

        public async Task<List<ResumeListItemResponse>> ListAsync(Guid userId, int limit, int offset, CancellationToken cancellationToken)
        {
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");

            var list = await _resumeRepository.GetByCandidateIdAsync(profile.Id, limit, offset, cancellationToken);

            var versionsByResume = await _dbContext.ResumeVersions
                .Where(v => list.Select(r => r.Id).Contains(v.ResumeId))
                .ToListAsync(cancellationToken);

            var result = new List<ResumeListItemResponse>();
            foreach (var r in list)
            {
                var currentVersionNumber = versionsByResume.FirstOrDefault(v => v.ResumeId == r.Id && v.Id == r.CurrentVersionId)?.VersionNumber;
                result.Add(new ResumeListItemResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Status = r.Status,
                    CurrentVersionId = r.CurrentVersionId,
                    CurrentVersionNumber = currentVersionNumber,
                    CreatedDate = r.CreatedDate,
                    UpdatedDate = r.UpdatedAt
                });
            }
            return result;
        }

        public async Task<List<ResumeListItemResponse>> ListAllAsync(int limit, int offset, CancellationToken cancellationToken)
        {
            var list = await _resumeRepository.GetAllAsync(limit, offset, cancellationToken);
            var resumeIds = list.Select(r => r.Id).ToList();
            var versionsByResume = await _dbContext.ResumeVersions
                .Where(v => resumeIds.Contains(v.ResumeId))
                .ToListAsync(cancellationToken);

            return list.Select(r => new ResumeListItemResponse
            {
                Id = r.Id,
                Name = r.Name,
                Status = r.Status,
                CurrentVersionId = r.CurrentVersionId,
                CurrentVersionNumber = versionsByResume.FirstOrDefault(v => v.ResumeId == r.Id && v.Id == r.CurrentVersionId)?.VersionNumber,
                CreatedDate = r.CreatedDate,
                UpdatedDate = r.UpdatedAt
            }).ToList();
        }

        public async Task<ResumeResponse?> GetByIdAsync(Guid resumeId, Guid userId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null || resume.IsDeleted) return null;

            // Verify ownership via candidate profile
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");

            if (resume.CandidateId != profile.Id) throw new UnauthorizedAccessException("Not owner");

            var resp = new ResumeResponse
            {
                Id = resume.Id,
                Name = resume.Name,
                Status = resume.Status,
                CurrentVersionId = resume.CurrentVersionId,
                CreatedDate = resume.CreatedDate,
                UpdatedDate = resume.UpdatedAt
            };

            var versions = await _resumeRepository.GetVersionsByResumeIdAsync(resume.Id);
            resp.Versions = versions.Select(v => new ResumeVersionResponse
            {
                Id = v.Id,
                VersionNumber = v.VersionNumber,
                CreatedDate = v.CreatedDate,
                ExtractedText = v.ExtractedText,
                StructuredData = v.StructuredData,
                IsCurrent = resp.CurrentVersionId == v.Id
            }).ToList();

            resp.CurrentVersionNumber = resp.Versions.FirstOrDefault(v => v.IsCurrent)?.VersionNumber;

            return resp;
        }

        public async Task<Stream?> DownloadAsync(Guid resumeId, Guid userId, CancellationToken cancellationToken)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null || resume.IsDeleted) return null;

            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");
            if (resume.CandidateId != profile.Id) throw new UnauthorizedAccessException("Not owner");

            return await _fileService.GetAsync(resume.FileId, cancellationToken);
        }

        public async Task DeleteAsync(Guid resumeId, Guid userId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null) throw new InvalidOperationException("Resume not found");
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");
            if (resume.CandidateId != profile.Id) throw new UnauthorizedAccessException("Not owner");

            resume.IsDeleted = true;
            resume.DeletedAt = DateTime.UtcNow;
            await _resumeRepository.UpdateAsync(resume);
        }

        public async Task RestoreAsync(Guid resumeId, Guid userId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null) throw new InvalidOperationException("Resume not found");
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");
            if (resume.CandidateId != profile.Id) throw new UnauthorizedAccessException("Not owner");

            resume.IsDeleted = false;
            resume.DeletedAt = null;
            await _resumeRepository.UpdateAsync(resume);
        }

        public async Task<ResumeVersionResponse> UploadVersionAsync(Guid resumeId, Stream fileStream, string fileName, string contentType, Guid userId, CancellationToken cancellationToken)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null) throw new InvalidOperationException("Resume not found");
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");
            if (resume.CandidateId != profile.Id) throw new UnauthorizedAccessException("Not owner");
            if (resume.IsDeleted) throw new InvalidOperationException("Resume is deleted");

            // Upload new file
            var upload = await _fileService.UploadAsync(fileStream, fileName, contentType, cancellationToken);
            ValidateFile(upload.FileName, upload.ContentType, upload.Size);

            // Determine next version number
            var versions = await _resumeRepository.GetVersionsByResumeIdAsync(resumeId);
            var next = versions.Any() ? versions.Max(v => v.VersionNumber) + 1 : 1;

            var version = new ResumeVersion
            {
                Id = Guid.NewGuid(),
                ResumeId = resumeId,
                VersionNumber = next,
                CreatedDate = DateTime.UtcNow,
                ExtractedText = null,
                StructuredData = null
            };

            // Save new version and set current
            await _resumeRepository.AddVersionAsync(version);
            resume.CurrentVersionId = version.Id;
            resume.FileId = upload.FileId;
            resume.Status = "PROCESSING";
            await _resumeRepository.UpdateAsync(resume);

            _taskQueue.Enqueue(ct => ProcessResumeAsync(upload.FileId, resume.Id, version.Id, ct));

            return new ResumeVersionResponse
            {
                Id = version.Id,
                VersionNumber = version.VersionNumber,
                CreatedDate = version.CreatedDate,
                ExtractedText = version.ExtractedText,
                StructuredData = version.StructuredData,
                IsCurrent = true
            };
        }

        public async Task<List<ResumeVersionResponse>> GetVersionsAsync(Guid resumeId, Guid userId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null) throw new InvalidOperationException("Resume not found");
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");
            if (resume.CandidateId != profile.Id) throw new UnauthorizedAccessException("Not owner");

            var versions = await _resumeRepository.GetVersionsByResumeIdAsync(resumeId);
            return versions.Select(v => new ResumeVersionResponse
            {
                Id = v.Id,
                VersionNumber = v.VersionNumber,
                CreatedDate = v.CreatedDate,
                ExtractedText = v.ExtractedText,
                StructuredData = v.StructuredData,
                IsCurrent = resume.CurrentVersionId == v.Id
            }).ToList();
        }

        public async Task<ResumeVersionResponse?> GetVersionAsync(Guid resumeId, Guid versionId, Guid userId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null) throw new InvalidOperationException("Resume not found");
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");
            if (resume.CandidateId != profile.Id) throw new UnauthorizedAccessException("Not owner");

            var version = await _resumeRepository.GetVersionByIdAsync(versionId);
            if (version == null || version.ResumeId != resumeId) throw new InvalidOperationException("Version not found");

            return new ResumeVersionResponse
            {
                Id = version.Id,
                VersionNumber = version.VersionNumber,
                CreatedDate = version.CreatedDate,
                ExtractedText = version.ExtractedText,
                StructuredData = version.StructuredData,
                IsCurrent = resume.CurrentVersionId == version.Id
            };
        }

        public async Task SetCurrentVersionAsync(Guid resumeId, Guid versionId, Guid userId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null) throw new InvalidOperationException("Resume not found");
            var profile = await _dbContext.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
            if (profile == null) throw new InvalidOperationException("Candidate profile not found");
            if (resume.CandidateId != profile.Id) throw new UnauthorizedAccessException("Not owner");

            var version = await _resumeRepository.GetVersionByIdAsync(versionId);
            if (version == null || version.ResumeId != resumeId) throw new InvalidOperationException("Version not found or does not belong to resume");

            resume.CurrentVersionId = version.Id;
            await _resumeRepository.UpdateAsync(resume);
        }

        public async Task<ResumeResponse?> GetByIdForReviewAsync(Guid resumeId, Guid reviewerUserId)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null || resume.IsDeleted) return null;

            var resp = new ResumeResponse
            {
                Id = resume.Id,
                Name = resume.Name,
                Status = resume.Status,
                CurrentVersionId = resume.CurrentVersionId,
                CreatedDate = resume.CreatedDate,
                UpdatedDate = resume.UpdatedAt
            };

            var versions = await _resumeRepository.GetVersionsByResumeIdAsync(resume.Id);
            resp.Versions = versions.Select(v => new ResumeVersionResponse
            {
                Id = v.Id,
                VersionNumber = v.VersionNumber,
                CreatedDate = v.CreatedDate,
                ExtractedText = v.ExtractedText,
                StructuredData = v.StructuredData,
                IsCurrent = resp.CurrentVersionId == v.Id
            }).ToList();

            resp.CurrentVersionNumber = resp.Versions.FirstOrDefault(v => v.IsCurrent)?.VersionNumber;

            return resp;
        }

        public async Task<Stream?> DownloadForReviewAsync(Guid resumeId, Guid reviewerUserId, CancellationToken cancellationToken)
        {
            var resume = await _resumeRepository.GetByIdAsync(resumeId);
            if (resume == null || resume.IsDeleted) return null;

            return await _fileService.GetAsync(resume.FileId, cancellationToken);
        }

        private async Task ProcessResumeAsync(Guid fileId, Guid resumeId, Guid versionId, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDBcontext>();

                using var fileStream = await fileService.GetAsync(fileId, cancellationToken);
                if (fileStream == null)
                    throw new FileNotFoundException($"Uploaded resume file {fileId} was not found.");

                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream, cancellationToken);

                var version = await dbContext.ResumeVersions.FirstOrDefaultAsync(v => v.Id == versionId, cancellationToken);
                if (version == null)
                    throw new InvalidOperationException($"Resume version {versionId} was not found.");

                version.ExtractedText = $"Extracted {memoryStream.Length} bytes";
                version.StructuredData = JsonSerializer.Serialize(new { parsed = true });

                var resume = await dbContext.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId, cancellationToken);
                if (resume == null)
                    throw new InvalidOperationException($"Resume {resumeId} was not found.");

                resume.Status = "COMPLETED";
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to process resume {ResumeId}, version {VersionId}.", resumeId, versionId);

                try
                {
                    using var failureScope = _scopeFactory.CreateScope();
                    var dbContext = failureScope.ServiceProvider.GetRequiredService<AppDBcontext>();
                    var resume = await dbContext.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId, CancellationToken.None);
                    if (resume != null)
                    {
                        resume.Status = "FAILED";
                        await dbContext.SaveChangesAsync(CancellationToken.None);
                    }
                }
                catch (Exception statusException)
                {
                    _logger.LogError(statusException, "Failed to mark resume {ResumeId} as failed.", resumeId);
                }
            }
        }
    }
}
