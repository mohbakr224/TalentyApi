using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IResumeService
    {
        Task<ResumeResponse> UploadAsync(Stream fileStream, string fileName, string contentType, string? name, Guid userId, CancellationToken cancellationToken);

        Task<List<ResumeListItemResponse>> ListAsync(Guid userId, int limit, int offset, CancellationToken cancellationToken);

        Task<List<ResumeListItemResponse>> ListAllAsync(int limit, int offset, CancellationToken cancellationToken);

        Task<ResumeResponse?> GetByIdAsync(Guid resumeId, Guid userId);

        Task<Stream?> DownloadAsync(Guid resumeId, Guid userId, CancellationToken cancellationToken);

        Task DeleteAsync(Guid resumeId, Guid userId);

        Task RestoreAsync(Guid resumeId, Guid userId);

        Task<ResumeVersionResponse> UploadVersionAsync(Guid resumeId, Stream fileStream, string fileName, string contentType, Guid userId, CancellationToken cancellationToken);

        Task<List<ResumeVersionResponse>> GetVersionsAsync(Guid resumeId, Guid userId);

        Task<ResumeVersionResponse?> GetVersionAsync(Guid resumeId, Guid versionId, Guid userId);

        Task SetCurrentVersionAsync(Guid resumeId, Guid versionId, Guid userId);

        Task<ResumeResponse?> GetByIdForReviewAsync(Guid resumeId, Guid reviewerUserId);

        Task<Stream?> DownloadForReviewAsync(Guid resumeId, Guid reviewerUserId, CancellationToken cancellationToken);
    }
}
