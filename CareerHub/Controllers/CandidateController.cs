using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/candidates")]
    public class CandidateController : ControllerBase
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
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
            var uid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(uid) || !Guid.TryParse(uid, out var userId))
                return Unauthorized();

            var updated = await _candidateService.UpdateMyProfileAsync(userId, request);
            return Ok(new { success = true, data = updated });
        }

        [Authorize, HttpGet("me/experiences")]
        public async Task<IActionResult> GetExperiences() => await ExecuteList(() => _candidateService.GetExperiencesAsync(GetUserId()));

        [Authorize, HttpPost("me/experiences")]
        public async Task<IActionResult> AddExperience([FromBody] ExperienceRequest request) => await Execute(() => _candidateService.AddExperienceAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/experiences/{id:guid}")]
        public async Task<IActionResult> DeleteExperience(Guid id) => await Delete(() => _candidateService.DeleteExperienceAsync(GetUserId(), id));

        [Authorize, HttpGet("me/education")]
        public async Task<IActionResult> GetEducation() => await ExecuteList(() => _candidateService.GetEducationAsync(GetUserId()));

        [Authorize, HttpPost("me/education")]
        public async Task<IActionResult> AddEducation([FromBody] EducationRequest request) => await Execute(() => _candidateService.AddEducationAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/education/{id:guid}")]
        public async Task<IActionResult> DeleteEducation(Guid id) => await Delete(() => _candidateService.DeleteEducationAsync(GetUserId(), id));

        [Authorize, HttpGet("me/skills")]
        public async Task<IActionResult> GetSkills() => await ExecuteList(() => _candidateService.GetSkillsAsync(GetUserId()));

        [Authorize, HttpPost("me/skills")]
        public async Task<IActionResult> AddSkill([FromBody] SkillRequest request) => await Execute(() => _candidateService.AddSkillAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/skills/{skillId:guid}")]
        public async Task<IActionResult> DeleteSkill(Guid skillId) => await Delete(() => _candidateService.DeleteSkillAsync(GetUserId(), skillId));

        [Authorize, HttpGet("me/languages")]
        public async Task<IActionResult> GetLanguages() => await ExecuteList(() => _candidateService.GetLanguagesAsync(GetUserId()));

        [Authorize, HttpPost("me/languages")]
        public async Task<IActionResult> AddLanguage([FromBody] LanguageRequest request) => await Execute(() => _candidateService.AddLanguageAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/languages/{id:guid}")]
        public async Task<IActionResult> DeleteLanguage(Guid id) => await Delete(() => _candidateService.DeleteLanguageAsync(GetUserId(), id));

        [Authorize, HttpGet("me/projects")]
        public async Task<IActionResult> GetProjects() => await ExecuteList(() => _candidateService.GetProjectsAsync(GetUserId()));

        [Authorize, HttpPost("me/projects")]
        public async Task<IActionResult> AddProject([FromBody] ProjectRequest request) => await Execute(() => _candidateService.AddProjectAsync(GetUserId(), request));

        [Authorize, HttpDelete("me/projects/{id:guid}")]
        public async Task<IActionResult> DeleteProject(Guid id) => await Delete(() => _candidateService.DeleteProjectAsync(GetUserId(), id));

        private Guid GetUserId()
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(value, out var userId) ? userId : throw new UnauthorizedAccessException("Invalid user");
        }

        private async Task<IActionResult> Execute<T>(Func<Task<T>> action)
        {
            try { return Ok(new { success = true, data = await action() }); }
            catch (UnauthorizedAccessException) { return Unauthorized(); }
            catch (InvalidOperationException exception) { return BadRequest(new { success = false, message = exception.Message }); }
        }

        private async Task<IActionResult> ExecuteList<T>(Func<Task<List<T>>> action) => await Execute(action);

        private async Task<IActionResult> Delete(Func<Task<bool>> action)
        {
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
