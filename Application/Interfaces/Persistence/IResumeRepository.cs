using Domain.Entities.Resumes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface IResumeRepository
    {
        Task AddAsync(Resume resume);

        Task<Resume?> GetByIdAsync(Guid id);

        Task UpdateAsync(Resume resume);

        Task<List<Resume>> GetByCandidateIdAsync(Guid candidateId, int limit, int offset, CancellationToken cancellationToken);

        Task<List<Resume>> GetAllAsync(int limit, int offset, CancellationToken cancellationToken);

        // Version operations
        Task AddVersionAsync(ResumeVersion version);

        Task<ResumeVersion?> GetVersionByIdAsync(Guid versionId);

        Task<List<ResumeVersion>> GetVersionsByResumeIdAsync(Guid resumeId);
    }
}
