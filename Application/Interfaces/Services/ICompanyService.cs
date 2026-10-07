using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICompanyService
    {
        Task<CompanyResponse> CreateAsync(CreateCompanyRequest request);

        Task<CompanyResponse?> GetByIdAsync(Guid id);

        Task<List<CompanyResponse>> SearchAsync(string? name, int limit, int offset, CancellationToken cancellationToken);

        Task<CompanyResponse?> GetCompanyForUserAsync(Guid userId);

        Task<List<CompanyMemberResponse>> GetMembersAsync(Guid companyId);

        Task<CompanyMemberResponse> AddMemberAsync(Guid companyId, AddCompanyMemberRequest request, Guid addedByUserId);

        Task<bool> RemoveMemberAsync(Guid companyId, Guid userId, Guid removedByUserId);

        Task<CompanyMemberResponse> UpdateMemberRoleAsync(Guid companyId, Guid userId, string newRole, Guid updatedByUserId);
    }
}
