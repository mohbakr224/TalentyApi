using Domain.Entities.Applications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface IApplicationRepository
    {
        Task AddAsync(Domain.Entities.Applications.Application application);

        Task<Domain.Entities.Applications.Application?> GetByIdAsync(Guid id);

        Task UpdateAsync(Domain.Entities.Applications.Application application);

        Task<List<Domain.Entities.Applications.Application>> GetByCandidateIdAsync(Guid candidateId, CancellationToken cancellationToken);

        Task<List<Domain.Entities.Applications.Application>> GetByJobIdAsync(Guid jobId, string? status, int limit, int offset, CancellationToken cancellationToken);

        Task<List<Domain.Entities.Applications.Application>> GetByCompanyIdAsync(Guid companyId, string? status, int limit, int offset, CancellationToken cancellationToken);
    }
}
