using Application.Interfaces.Services;
using Application.DTOs.Request;
using Application.DTOs.Response;
using infrastructure.Persistence;
using Domain.Entities.Candidates;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace infrastructure.Services.Candidate
{
    public class CandidateService : ICandidateService
    {
        private readonly AppDBcontext _dbContext;

        public CandidateService(AppDBcontext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CandidateProfileResponse?> GetMyProfileAsync(Guid userId)
        {
            var profile = await _dbContext.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);

            if (profile == null)
                return null;

            return new CandidateProfileResponse
            {
                UserId = profile.UserId,
                Headline = profile.Headline,
                About = profile.About,
                Location = profile.Location,
                CareerLevel = profile.CareerLevel,
                TargetRole = profile.TargetRole,
                YearsOfExperience = profile.YearsOfExperience,
                WorkPreference = profile.WorkPreference,
                Availability = profile.Availability,
                SalaryExpectation = profile.SalaryExpectation,
                IsPublic = profile.IsPublic
            };
        }

        private async Task<CandidateProfile> GetProfileAsync(Guid userId)
        {
            var profile = await _dbContext.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);

            if (profile == null)
                throw new InvalidOperationException("Candidate profile not found");

            return profile;
        }

        public async Task<List<ExperienceResponse>> GetExperiencesAsync(Guid userId)
        {
            var profile = await GetProfileAsync(userId);
            return await _dbContext.CandidateExperiences.Where(x => x.CandidateId == profile.Id)
                .OrderByDescending(x => x.StartDate).Select(x => new ExperienceResponse
                {
                    Id = x.Id, CompanyName = x.CompanyName, JobTitle = x.JobTitle,
                    Description = x.Description, StartDate = x.StartDate, EndDate = x.EndDate, IsCurrent = x.IsCurrent
                }).ToListAsync();
        }

        public async Task<ExperienceResponse> AddExperienceAsync(Guid userId, ExperienceRequest request)
        {
            var profile = await GetProfileAsync(userId);
            var entity = new CandidateExperience
            {
                Id = Guid.NewGuid(), CandidateId = profile.Id, CompanyName = request.CompanyName,
                JobTitle = request.JobTitle, Description = request.Description, StartDate = request.StartDate,
                EndDate = request.EndDate, IsCurrent = request.IsCurrent, CreatedDate = DateTime.UtcNow
            };
            _dbContext.CandidateExperiences.Add(entity);
            await _dbContext.SaveChangesAsync();
            return new ExperienceResponse { Id = entity.Id, CompanyName = entity.CompanyName, JobTitle = entity.JobTitle, Description = entity.Description, StartDate = entity.StartDate, EndDate = entity.EndDate, IsCurrent = entity.IsCurrent };
        }

        public async Task<bool> DeleteExperienceAsync(Guid userId, Guid id)
        {
            var profile = await GetProfileAsync(userId);
            var entity = await _dbContext.CandidateExperiences.FirstOrDefaultAsync(x => x.Id == id && x.CandidateId == profile.Id);
            if (entity == null) return false;
            _dbContext.CandidateExperiences.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<EducationResponse>> GetEducationAsync(Guid userId)
        {
            var profile = await GetProfileAsync(userId);
            return await _dbContext.CandidateEducations.Where(x => x.CandidateId == profile.Id)
                .OrderByDescending(x => x.EndDate).Select(x => new EducationResponse
                {
                    Id = x.Id, Institution = x.Institution, Degree = x.Degree, FieldOfStudy = x.FieldOfStudy,
                    StartDate = x.StartDate, EndDate = x.EndDate, GPA = x.GPA
                }).ToListAsync();
        }

        public async Task<EducationResponse> AddEducationAsync(Guid userId, EducationRequest request)
        {
            var profile = await GetProfileAsync(userId);
            var entity = new CandidateEducation
            {
                Id = Guid.NewGuid(), CandidateId = profile.Id, Institution = request.Institution, Degree = request.Degree,
                FieldOfStudy = request.FieldOfStudy, StartDate = request.StartDate, EndDate = request.EndDate,
                GPA = request.GPA, CreatedDate = DateTime.UtcNow
            };
            _dbContext.CandidateEducations.Add(entity);
            await _dbContext.SaveChangesAsync();
            return new EducationResponse { Id = entity.Id, Institution = entity.Institution, Degree = entity.Degree, FieldOfStudy = entity.FieldOfStudy, StartDate = entity.StartDate, EndDate = entity.EndDate, GPA = entity.GPA };
        }

        public async Task<bool> DeleteEducationAsync(Guid userId, Guid id)
        {
            var profile = await GetProfileAsync(userId);
            var entity = await _dbContext.CandidateEducations.FirstOrDefaultAsync(x => x.Id == id && x.CandidateId == profile.Id);
            if (entity == null) return false;
            _dbContext.CandidateEducations.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<SkillResponse>> GetSkillsAsync(Guid userId)
        {
            var profile = await GetProfileAsync(userId);
            return await _dbContext.CandidateSkills.Where(x => x.CandidateId == profile.Id).Include(x => x.Skill)
                .Select(x => new SkillResponse { SkillId = x.SkillId, SkillName = x.Skill.Name, ProficiencyLevel = x.ProficiencyLevel, YearsOfExperience = x.YearsOfExperience }).ToListAsync();
        }

        public async Task<SkillResponse> AddSkillAsync(Guid userId, SkillRequest request)
        {
            var profile = await GetProfileAsync(userId);
            var skill = await _dbContext.Skills.FirstOrDefaultAsync(x => x.Id == request.SkillId);
            if (skill == null) throw new InvalidOperationException("Skill not found");
            var existing = await _dbContext.CandidateSkills.FirstOrDefaultAsync(x => x.CandidateId == profile.Id && x.SkillId == request.SkillId);
            if (existing != null) throw new InvalidOperationException("Skill already added");
            var entity = new CandidateSkill { CandidateId = profile.Id, SkillId = request.SkillId, ProficiencyLevel = request.ProficiencyLevel, YearsOfExperience = request.YearsOfExperience };
            _dbContext.CandidateSkills.Add(entity);
            await _dbContext.SaveChangesAsync();
            return new SkillResponse { SkillId = entity.SkillId, SkillName = skill.Name, ProficiencyLevel = entity.ProficiencyLevel, YearsOfExperience = entity.YearsOfExperience };
        }

        public async Task<bool> DeleteSkillAsync(Guid userId, Guid skillId)
        {
            var profile = await GetProfileAsync(userId);
            var entity = await _dbContext.CandidateSkills.FirstOrDefaultAsync(x => x.CandidateId == profile.Id && x.SkillId == skillId);
            if (entity == null) return false;
            _dbContext.CandidateSkills.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<LanguageResponse>> GetLanguagesAsync(Guid userId)
        {
            var profile = await GetProfileAsync(userId);
            return await _dbContext.CandidateLanguages.Where(x => x.CandidateId == profile.Id)
                .Select(x => new LanguageResponse { Id = x.Id, Language = x.Language, ProficiencyLevel = x.ProficiencyLevel }).ToListAsync();
        }

        public async Task<LanguageResponse> AddLanguageAsync(Guid userId, LanguageRequest request)
        {
            var profile = await GetProfileAsync(userId);
            var entity = new CandidateLanguage { Id = Guid.NewGuid(), CandidateId = profile.Id, Language = request.Language, ProficiencyLevel = request.ProficiencyLevel, CreatedDate = DateTime.UtcNow };
            _dbContext.CandidateLanguages.Add(entity);
            await _dbContext.SaveChangesAsync();
            return new LanguageResponse { Id = entity.Id, Language = entity.Language, ProficiencyLevel = entity.ProficiencyLevel };
        }

        public async Task<bool> DeleteLanguageAsync(Guid userId, Guid id)
        {
            var profile = await GetProfileAsync(userId);
            var entity = await _dbContext.CandidateLanguages.FirstOrDefaultAsync(x => x.Id == id && x.CandidateId == profile.Id);
            if (entity == null) return false;
            _dbContext.CandidateLanguages.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProjectResponse>> GetProjectsAsync(Guid userId)
        {
            var profile = await GetProfileAsync(userId);
            return await _dbContext.CandidateProjects.Where(x => x.CandidateId == profile.Id)
                .OrderByDescending(x => x.StartDate).Select(x => new ProjectResponse
                {
                    Id = x.Id, Name = x.Name, Description = x.Description, RepositoryUrl = x.RepositoryUrl,
                    ProjectUrl = x.ProjectUrl, StartDate = x.StartDate, EndDate = x.EndDate
                }).ToListAsync();
        }

        public async Task<ProjectResponse> AddProjectAsync(Guid userId, ProjectRequest request)
        {
            var profile = await GetProfileAsync(userId);
            var entity = new CandidateProject { Id = Guid.NewGuid(), CandidateId = profile.Id, Name = request.Name, Description = request.Description, RepositoryUrl = request.RepositoryUrl, ProjectUrl = request.ProjectUrl, StartDate = request.StartDate, EndDate = request.EndDate, CreatedDate = DateTime.UtcNow };
            _dbContext.CandidateProjects.Add(entity);
            await _dbContext.SaveChangesAsync();
            return new ProjectResponse { Id = entity.Id, Name = entity.Name, Description = entity.Description, RepositoryUrl = entity.RepositoryUrl, ProjectUrl = entity.ProjectUrl, StartDate = entity.StartDate, EndDate = entity.EndDate };
        }

        public async Task<bool> DeleteProjectAsync(Guid userId, Guid id)
        {
            var profile = await GetProfileAsync(userId);
            var entity = await _dbContext.CandidateProjects.FirstOrDefaultAsync(x => x.Id == id && x.CandidateId == profile.Id);
            if (entity == null) return false;
            _dbContext.CandidateProjects.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<CandidateProfileResponse> UpdateMyProfileAsync(Guid userId, UpdateCandidateProfileRequest request)
        {
            var profile = await _dbContext.CandidateProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);

            if (profile == null)
            {
                profile = new CandidateProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.CandidateProfiles.Add(profile);
            }

            profile.Headline = request.Headline ?? profile.Headline;
            profile.About = request.About ?? profile.About;
            profile.Location = request.Location ?? profile.Location;
            profile.CareerLevel = request.CareerLevel ?? profile.CareerLevel;
            profile.TargetRole = request.TargetRole ?? profile.TargetRole;
            profile.YearsOfExperience = request.YearsOfExperience ?? profile.YearsOfExperience;
            profile.WorkPreference = request.WorkPreference ?? profile.WorkPreference;
            profile.Availability = request.Availability ?? profile.Availability;
            profile.SalaryExpectation = request.SalaryExpectation ?? profile.SalaryExpectation;
            if (request.IsPublic.HasValue)
                profile.IsPublic = request.IsPublic.Value;

            await _dbContext.SaveChangesAsync();

            return new CandidateProfileResponse
            {
                UserId = profile.UserId,
                Headline = profile.Headline,
                About = profile.About,
                Location = profile.Location,
                CareerLevel = profile.CareerLevel,
                TargetRole = profile.TargetRole,
                YearsOfExperience = profile.YearsOfExperience,
                WorkPreference = profile.WorkPreference,
                Availability = profile.Availability,
                SalaryExpectation = profile.SalaryExpectation,
                IsPublic = profile.IsPublic
            };
        }

        public async Task<CandidateDetailedProfileResponse?> GetCandidateByIdAsync(Guid candidateIdOrUserId)
        {
            var profile = await _dbContext.CandidateProfiles
                .Include(p => p.User)
                .Include(p => p.Experiences)
                .Include(p => p.Education)
                .Include(p => p.Skills).ThenInclude(s => s.Skill)
                .Include(p => p.Languages)
                .Include(p => p.Projects)
                .FirstOrDefaultAsync(p => (p.Id == candidateIdOrUserId || p.UserId == candidateIdOrUserId) && !p.IsDeleted);

            if (profile == null) return null;

            return new CandidateDetailedProfileResponse
            {
                CandidateId = profile.Id,
                UserId = profile.UserId,
                FirstName = profile.User?.FirstName ?? string.Empty,
                LastName = profile.User?.LastName ?? string.Empty,
                Email = profile.User?.Email ?? string.Empty,
                Headline = profile.Headline,
                About = profile.About,
                Location = profile.Location,
                CareerLevel = profile.CareerLevel,
                TargetRole = profile.TargetRole,
                YearsOfExperience = profile.YearsOfExperience,
                WorkPreference = profile.WorkPreference,
                Availability = profile.Availability,
                SalaryExpectation = profile.SalaryExpectation,
                IsPublic = profile.IsPublic,
                Experiences = profile.Experiences.OrderByDescending(x => x.StartDate).Select(x => new ExperienceResponse
                {
                    Id = x.Id, CompanyName = x.CompanyName, JobTitle = x.JobTitle, Description = x.Description,
                    StartDate = x.StartDate, EndDate = x.EndDate, IsCurrent = x.IsCurrent
                }).ToList(),
                Education = profile.Education.OrderByDescending(x => x.EndDate).Select(x => new EducationResponse
                {
                    Id = x.Id, Institution = x.Institution, Degree = x.Degree, FieldOfStudy = x.FieldOfStudy,
                    StartDate = x.StartDate, EndDate = x.EndDate, GPA = x.GPA
                }).ToList(),
                Skills = profile.Skills.Select(x => new SkillResponse
                {
                    SkillId = x.SkillId, SkillName = x.Skill?.Name, ProficiencyLevel = x.ProficiencyLevel, YearsOfExperience = x.YearsOfExperience
                }).ToList(),
                Languages = profile.Languages.Select(x => new LanguageResponse
                {
                    Id = x.Id, Language = x.Language, ProficiencyLevel = x.ProficiencyLevel
                }).ToList(),
                Projects = profile.Projects.OrderByDescending(x => x.StartDate).Select(x => new ProjectResponse
                {
                    Id = x.Id, Name = x.Name, Description = x.Description, RepositoryUrl = x.RepositoryUrl,
                    ProjectUrl = x.ProjectUrl, StartDate = x.StartDate, EndDate = x.EndDate
                }).ToList()
            };
        }

        public async Task<List<CandidateSearchResponse>> SearchCandidatesAsync(CandidateSearchFilter filter, CancellationToken cancellationToken)
        {
            var query = _dbContext.CandidateProfiles
                .Include(p => p.User)
                .Include(p => p.Skills).ThenInclude(s => s.Skill)
                .Where(p => !p.IsDeleted && p.IsPublic);

            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                var q = filter.Query.Trim();
                query = query.Where(p =>
                    EF.Functions.ILike(p.User.FirstName, $"%{q}%") ||
                    EF.Functions.ILike(p.User.LastName, $"%{q}%") ||
                    EF.Functions.ILike(p.User.Email, $"%{q}%") ||
                    (p.Headline != null && EF.Functions.ILike(p.Headline, $"%{q}%")) ||
                    (p.About != null && EF.Functions.ILike(p.About, $"%{q}%")) ||
                    (p.TargetRole != null && EF.Functions.ILike(p.TargetRole, $"%{q}%")));
            }

            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                query = query.Where(p => p.Location != null && EF.Functions.ILike(p.Location, $"%{filter.Location}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.CareerLevel))
            {
                query = query.Where(p => p.CareerLevel != null && EF.Functions.ILike(p.CareerLevel, $"%{filter.CareerLevel}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.TargetRole))
            {
                query = query.Where(p => p.TargetRole != null && EF.Functions.ILike(p.TargetRole, $"%{filter.TargetRole}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.WorkPreference))
            {
                query = query.Where(p => p.WorkPreference != null && EF.Functions.ILike(p.WorkPreference, $"%{filter.WorkPreference}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.Availability))
            {
                query = query.Where(p => p.Availability != null && EF.Functions.ILike(p.Availability, $"%{filter.Availability}%"));
            }

            if (filter.MinExperience.HasValue)
            {
                query = query.Where(p => p.YearsOfExperience.HasValue && p.YearsOfExperience.Value >= filter.MinExperience.Value);
            }

            if (filter.MaxExperience.HasValue)
            {
                query = query.Where(p => p.YearsOfExperience.HasValue && p.YearsOfExperience.Value <= filter.MaxExperience.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Skill))
            {
                query = query.Where(p => p.Skills.Any(s => s.Skill != null && EF.Functions.ILike(s.Skill.Name, $"%{filter.Skill}%")));
            }

            var list = await query
                .OrderByDescending(p => p.YearsOfExperience ?? 0)
                .Skip(filter.Offset)
                .Take(filter.Limit)
                .ToListAsync(cancellationToken);

            return list.Select(p => new CandidateSearchResponse
            {
                CandidateId = p.Id,
                UserId = p.UserId,
                FullName = $"{p.User?.FirstName} {p.User?.LastName}".Trim(),
                Email = p.User?.Email ?? string.Empty,
                Headline = p.Headline,
                Location = p.Location,
                CareerLevel = p.CareerLevel,
                TargetRole = p.TargetRole,
                YearsOfExperience = p.YearsOfExperience,
                WorkPreference = p.WorkPreference,
                Availability = p.Availability,
                SalaryExpectation = p.SalaryExpectation,
                Skills = p.Skills.Where(s => s.Skill != null).Select(s => s.Skill!.Name).ToList()
            }).ToList();
        }
    }
}
