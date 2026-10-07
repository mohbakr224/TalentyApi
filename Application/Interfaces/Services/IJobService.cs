using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IJobService
    {
        Task<JobResponse> CreateAsync(CreateJobRequest request);

        Task<JobResponse?> GetByIdAsync(Guid id);

        Task<JobResponse> UpdateAsync(Guid id, UpdateJobRequest request, Guid userId);

        Task<JobResponse> PublishAsync(Guid id, Guid userId);

        Task<JobResponse> CloseAsync(Guid id, Guid userId);

        Task<List<JobResponse>> SearchAsync(string? title, int limit, int offset, CancellationToken cancellationToken);

        Task<List<JobResponse>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);
    }
}
