using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Services;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using CompanyEntity = Domain.Entities.Companies.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace infrastructure.Services.Company
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDBcontext _dbContext;

        public CompanyService(AppDBcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CompanyResponse> CreateAsync(CreateCompanyRequest request)
        {
            var company = new CompanyEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Industry = request.Industry,
                Website = request.Website,
                Location = request.Location,
                CreatedDate = DateTime.UtcNow
            };

            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            return new CompanyResponse
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                Industry = company.Industry,
                Website = company.Website,
                Location = company.Location
            };
        }

        public async Task<CompanyResponse?> GetByIdAsync(Guid id)
        {
            var c = await _dbContext.Companies
                .FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return null;
            return new CompanyResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Industry = c.Industry,
                Website = c.Website,
                Location = c.Location
            };
        }

        public async Task<List<CompanyResponse>> SearchAsync(string? name, int limit, int offset, CancellationToken cancellationToken)
        {
            var q = _dbContext.Companies.AsQueryable();
            if (!string.IsNullOrEmpty(name))
                q = q.Where(c => EF.Functions.ILike(c.Name, $"%{name}%"));

            var list = await q.Skip(offset).Take(limit).ToListAsync(cancellationToken);
            return list.Select(c => new CompanyResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Industry = c.Industry,
                Website = c.Website,
                Location = c.Location
            }).ToList();
        }

        public async Task<CompanyResponse?> GetCompanyForUserAsync(Guid userId)
        {
            var membership = await _dbContext.CompanyMembers
                .Include(m => m.Company)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.IsActive && !m.Company.IsDeleted);

            if (membership?.Company == null)
                return null;

            var c = membership.Company;
            return new CompanyResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Industry = c.Industry,
                Website = c.Website,
                Location = c.Location
            };
        }

        public async Task<List<CompanyMemberResponse>> GetMembersAsync(Guid companyId)
        {
            var members = await _dbContext.CompanyMembers
                .Include(m => m.User)
                .Where(m => m.CompanyId == companyId)
                .OrderBy(m => m.CreatedDate)
                .ToListAsync();

            return members.Select(m => new CompanyMemberResponse
            {
                Id = m.Id,
                CompanyId = m.CompanyId,
                UserId = m.UserId,
                FullName = $"{m.User?.FirstName} {m.User?.LastName}".Trim(),
                Email = m.User?.Email ?? string.Empty,
                Role = m.Role,
                IsActive = m.IsActive,
                CreatedDate = m.CreatedDate
            }).ToList();
        }

        public async Task<CompanyMemberResponse> AddMemberAsync(Guid companyId, AddCompanyMemberRequest request, Guid addedByUserId)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == companyId && !c.IsDeleted);
            if (company == null)
                throw new InvalidOperationException("Company not found");

            Domain.Entities.Identity.User? user = null;
            if (request.UserId.HasValue)
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId.Value && !u.IsDeleted);
            }
            else if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);
            }

            if (user == null)
                throw new InvalidOperationException("User not found to add as member");

            var existing = await _dbContext.CompanyMembers
                .FirstOrDefaultAsync(m => m.CompanyId == companyId && m.UserId == user.Id);

            var roleStr = string.IsNullOrWhiteSpace(request.Role) ? "HR" : request.Role.ToUpperInvariant();

            if (existing != null)
            {
                existing.IsActive = true;
                existing.Role = roleStr;
                existing.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return new CompanyMemberResponse
                {
                    Id = existing.Id,
                    CompanyId = existing.CompanyId,
                    UserId = existing.UserId,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Email = user.Email,
                    Role = existing.Role,
                    IsActive = existing.IsActive,
                    CreatedDate = existing.CreatedDate
                };
            }

            var member = new Domain.Entities.Companies.CompanyMember
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                UserId = user.Id,
                Role = roleStr,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            // Also grant corresponding system role if exists
            var targetRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleStr);
            if (targetRole != null)
            {
                var hasUserRole = await _dbContext.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == targetRole.Id);
                if (!hasUserRole)
                {
                    _dbContext.UserRoles.Add(new Domain.Entities.Identity.UserRole
                    {
                        UserId = user.Id,
                        RoleId = targetRole.Id
                    });
                }
            }

            _dbContext.CompanyMembers.Add(member);
            await _dbContext.SaveChangesAsync();

            return new CompanyMemberResponse
            {
                Id = member.Id,
                CompanyId = member.CompanyId,
                UserId = member.UserId,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                Role = member.Role,
                IsActive = member.IsActive,
                CreatedDate = member.CreatedDate
            };
        }

        public async Task<bool> RemoveMemberAsync(Guid companyId, Guid userId, Guid removedByUserId)
        {
            var member = await _dbContext.CompanyMembers
                .FirstOrDefaultAsync(m => m.CompanyId == companyId && m.UserId == userId);

            if (member == null) return false;

            member.IsActive = false;
            member.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<CompanyMemberResponse> UpdateMemberRoleAsync(Guid companyId, Guid userId, string newRole, Guid updatedByUserId)
        {
            var member = await _dbContext.CompanyMembers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.CompanyId == companyId && m.UserId == userId);

            if (member == null)
                throw new InvalidOperationException("Member not found in company");

            var roleStr = string.IsNullOrWhiteSpace(newRole) ? "HR" : newRole.ToUpperInvariant();
            member.Role = roleStr;
            member.UpdatedAt = DateTime.UtcNow;

            var targetRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleStr);
            if (targetRole != null)
            {
                var hasUserRole = await _dbContext.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == targetRole.Id);
                if (!hasUserRole)
                {
                    _dbContext.UserRoles.Add(new Domain.Entities.Identity.UserRole
                    {
                        UserId = userId,
                        RoleId = targetRole.Id
                    });
                }
            }

            await _dbContext.SaveChangesAsync();

            return new CompanyMemberResponse
            {
                Id = member.Id,
                CompanyId = member.CompanyId,
                UserId = member.UserId,
                FullName = $"{member.User?.FirstName} {member.User?.LastName}".Trim(),
                Email = member.User?.Email ?? string.Empty,
                Role = member.Role,
                IsActive = member.IsActive,
                CreatedDate = member.CreatedDate
            };
        }
    }
}
