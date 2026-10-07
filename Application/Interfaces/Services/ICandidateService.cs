using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICandidateService
    {
        Task<CandidateProfileResponse?> GetMyProfileAsync(Guid userId);

        Task<CandidateProfileResponse> UpdateMyProfileAsync(Guid userId, UpdateCandidateProfileRequest request);

        Task<List<ExperienceResponse>> GetExperiencesAsync(Guid userId);
        Task<ExperienceResponse> AddExperienceAsync(Guid userId, ExperienceRequest request);
        Task<bool> DeleteExperienceAsync(Guid userId, Guid id);

        Task<List<EducationResponse>> GetEducationAsync(Guid userId);
        Task<EducationResponse> AddEducationAsync(Guid userId, EducationRequest request);
        Task<bool> DeleteEducationAsync(Guid userId, Guid id);

        Task<List<SkillResponse>> GetSkillsAsync(Guid userId);
        Task<SkillResponse> AddSkillAsync(Guid userId, SkillRequest request);
        Task<bool> DeleteSkillAsync(Guid userId, Guid skillId);

        Task<List<LanguageResponse>> GetLanguagesAsync(Guid userId);
        Task<LanguageResponse> AddLanguageAsync(Guid userId, LanguageRequest request);
        Task<bool> DeleteLanguageAsync(Guid userId, Guid id);

        Task<List<ProjectResponse>> GetProjectsAsync(Guid userId);
        Task<ProjectResponse> AddProjectAsync(Guid userId, ProjectRequest request);
        Task<bool> DeleteProjectAsync(Guid userId, Guid id);

        Task<CandidateDetailedProfileResponse?> GetCandidateByIdAsync(Guid candidateIdOrUserId);
        Task<List<CandidateSearchResponse>> SearchCandidatesAsync(CandidateSearchFilter filter, CancellationToken cancellationToken);
    }
}
