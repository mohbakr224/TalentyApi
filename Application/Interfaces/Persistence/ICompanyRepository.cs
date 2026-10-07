using Domain.Entities.Companies;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Persistence
{
    public interface ICompanyRepository
    {
        Task AddAsync(Company company);

        Task<Company?> GetByIdAsync(Guid id);

        Task UpdateAsync(Company company);

        Task<List<Company>> SearchAsync(string? name, int limit, int offset, CancellationToken cancellationToken);
    }
}
