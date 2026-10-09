using Application.DTOs.Request;
using Application.Interfaces.Services;
using Application.Interfaces.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/candidates")]
    [Authorize(Roles = "CANDIDATE,ADMIN")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;
        private readonly IPermissionChecker _permissionChecker;

        public CandidateController(ICandidateService candidateService, IPermissionChecker permissionChecker)
        {
            _candidateService = candidateService;
            _permissionChecker = permissionChecker;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.profile.read")) return Forbid();
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(uid) || !Guid.TryParse(uid, out var userId))
                return Unauthorized();

            var profile = await _candidateService.GetMyProfileAsync(userId);
            if (profile == null)
                return NotFound();

            return Ok(new { success = true, data = profile });
        }

        [Authorize]
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateCandidateProfileRequest request)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.profile.update")) return Forbid();
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(uid) || !Guid.TryParse(uid, out var userId))
                return Unauthorized();

            var updated = await _candidateService.UpdateMyProfileAsync(userId, request);
            return Ok(new { success = true, data = updated });
        }

        [Authorize, HttpGet("me/experiences")]
        public async Task<IActionResult> GetExperiences() => await ExecuteList("candidate.profile.read", () => _candidateService.GetExperiencesAsync(GetUserId()));

        [Authorize, HttpPost("me/experiences")]
        public async Task<IActionResult> AddExperience([FromBody] ExperienceRequest request) => await Execute("candidate.profile.update", () => _candidateService.AddExperienceAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/experiences/{id:guid}")]
        public async Task<IActionResult> DeleteExperience(Guid id) => await Delete("candidate.profile.update", () => _candidateService.DeleteExperienceAsync(GetUserId(), id));

        [Authorize, HttpGet("me/education")]
        public async Task<IActionResult> GetEducation() => await ExecuteList("candidate.profile.read", () => _candidateService.GetEducationAsync(GetUserId()));

        [Authorize, HttpPost("me/education")]
        public async Task<IActionResult> AddEducation([FromBody] EducationRequest request) => await Execute("candidate.profile.update", () => _candidateService.AddEducationAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/education/{id:guid}")]
        public async Task<IActionResult> DeleteEducation(Guid id) => await Delete("candidate.profile.update", () => _candidateService.DeleteEducationAsync(GetUserId(), id));

        [Authorize, HttpGet("me/skills")]
        public async Task<IActionResult> GetSkills() => await ExecuteList("candidate.profile.read", () => _candidateService.GetSkillsAsync(GetUserId()));

        [Authorize, HttpPost("me/skills")]
        public async Task<IActionResult> AddSkill([FromBody] SkillRequest request) => await Execute("candidate.profile.update", () => _candidateService.AddSkillAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/skills/{skillId:guid}")]
        public async Task<IActionResult> DeleteSkill(Guid skillId) => await Delete("candidate.profile.update", () => _candidateService.DeleteSkillAsync(GetUserId(), skillId));

        [Authorize, HttpGet("me/languages")]
        public async Task<IActionResult> GetLanguages() => await ExecuteList("candidate.profile.read", () => _candidateService.GetLanguagesAsync(GetUserId()));

        [Authorize, HttpPost("me/languages")]
        public async Task<IActionResult> AddLanguage([FromBody] LanguageRequest request) => await Execute("candidate.profile.update", () => _candidateService.AddLanguageAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/languages/{id:guid}")]
        public async Task<IActionResult> DeleteLanguage(Guid id) => await Delete("candidate.profile.update", () => _candidateService.DeleteLanguageAsync(GetUserId(), id));

        [Authorize, HttpGet("me/projects")]
        public async Task<IActionResult> GetProjects() => await ExecuteList("candidate.profile.read", () => _candidateService.GetProjectsAsync(GetUserId()));

        [Authorize, HttpPost("me/projects")]
        public async Task<IActionResult> AddProject([FromBody] ProjectRequest request) => await Execute("candidate.profile.update", () => _candidateService.AddProjectAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/projects/{id:guid}")]
        public async Task<IActionResult> DeleteProject(Guid id) => await Delete("candidate.profile.update", () => _candidateService.DeleteProjectAsync(GetUserId(), id));

        private Guid GetUserId()
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out var userId) ? userId : throw new UnauthorizedAccessException("Invalid user");
        }

        private async Task<IActionResult> Execute<T>(string permission, Func<Task<T>> action)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, permission)) return Forbid();
            try { return Ok(new { success = true, data = await action() }); }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (InvalidOperationException exception) { return BadRequest(new { success = false, message = exception.Message }); }
        }

        private async Task<IActionResult> ExecuteList<T>(string permission, Func<Task<List<T>>> action) => await Execute(permission, action);

        private async Task<IActionResult> Delete(string permission, Func<Task<bool>> action)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, permission)) return Forbid();
            try
            {
                if (!await action()) return NotFound(new { success = false, message = "Resource not found" });
                return Ok(new { success = true });
            }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (InvalidOperationException exception) { return BadRequest(new { success = false, message = exception.Message }); }
        }
    }
}
