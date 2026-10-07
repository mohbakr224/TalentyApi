using Domain.Entities.Jobs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface IJobRepository
    {
        Task AddAsync(Job job);

        Task<Job?> GetByIdAsync(Guid id);

        Task UpdateAsync(Job job);

        Task<List<Job>> SearchAsync(string? title, string? location, int limit, int offset, CancellationToken cancellationToken);

        Task<List<Job>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);
    }
}
