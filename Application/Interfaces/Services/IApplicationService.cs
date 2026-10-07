using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IApplicationService
    {
        Task<ApplicationResponse> CreateAsync(CreateApplicationRequest request, Guid candidateId);

        Task<ApplicationResponse?> GetByIdAsync(Guid id);

        Task<List<ApplicationResponse>> GetByCandidateIdAsync(Guid candidateId, CancellationToken cancellationToken);

        Task<List<ApplicationResponse>> GetByJobIdAsync(Guid jobId, string? status, int limit, int offset, CancellationToken cancellationToken);

        Task<List<ApplicationResponse>> GetByCompanyIdAsync(Guid companyId, string? status, int limit, int offset, CancellationToken cancellationToken);

        Task<ApplicationResponse> ChangeStatusAsync(Guid applicationId, string newStatus, Guid changedBy, string? reason = null);

        Task<ApplicationResponse> ShortlistAsync(Guid applicationId, Guid changedBy, string? reason = null);

        Task<ApplicationResponse> RejectAsync(Guid applicationId, Guid changedBy, string? reason = null);
    }
}
