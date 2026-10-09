using Application.DTOs.Response;
using Application.Interfaces.Services;
using Application.Interfaces.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/resumes")]
    [Authorize(Roles = "CANDIDATE,ADMIN")]
    public class ResumesController : ControllerBase
    {
        private readonly IResumeService _resumeService;
        private readonly IPermissionChecker _permissionChecker;

        public ResumesController(IResumeService resumeService, IPermissionChecker permissionChecker)
        {
            _resumeService = resumeService;
            _permissionChecker = permissionChecker;
        }

        [HttpPost]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
            [FromForm] ResumeUploadRequest request,
            CancellationToken cancellationToken)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.upload")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            if (request.File == null)
                return BadRequest(new { success = false, message = "Resume file is required" });

            try
            {
                await using var stream = request.File.OpenReadStream();
                var resume = await _resumeService.UploadAsync(
                    stream,
                    request.File.FileName,
                    request.File.ContentType,
                    request.Name,
                    userId,
                    cancellationToken);

                return Ok(new { success = true, data = resume });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] int limit = 20,
            [FromQuery] int offset = 0,
            CancellationToken cancellationToken = default)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            limit = Math.Clamp(limit, 1, 100);
            offset = Math.Max(offset, 0);

            try
            {
                var resumes = await _resumeService.ListAsync(userId, limit, offset, cancellationToken);
                return Ok(new { success = true, data = resumes });
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        [HttpGet("{resumeId:guid}")]
        public async Task<IActionResult> Get(Guid resumeId)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            try
            {
                var resume = await _resumeService.GetByIdAsync(resumeId, userId);
                return resume == null
                    ? NotFound(new { success = false, message = "Resume not found" })
                    : Ok(new { success = true, data = resume });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        [HttpGet("{resumeId:guid}/download")]
        public async Task<IActionResult> Download(Guid resumeId, CancellationToken cancellationToken)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            try
            {
                var stream = await _resumeService.DownloadAsync(resumeId, userId, cancellationToken);
                return stream == null
                    ? NotFound(new { success = false, message = "Resume file not found" })
                    : File(stream, "application/pdf", $"resume-{resumeId}.pdf");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        [HttpGet("{resumeId:guid}/preview")]
        public async Task<IActionResult> Preview(Guid resumeId, CancellationToken cancellationToken)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            try
            {
                var resume = await _resumeService.GetByIdAsync(resumeId, userId);
                if (resume == null)
                    return NotFound(new { success = false, message = "Resume not found" });

                var stream = await _resumeService.DownloadAsync(resumeId, userId, cancellationToken);
                return stream == null
                    ? NotFound(new { success = false, message = "Resume file not found" })
                    : File(stream, "application/pdf");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        [HttpDelete("{resumeId:guid}")]
        public async Task<IActionResult> Delete(Guid resumeId)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.upload")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            try
            {
                await _resumeService.DeleteAsync(resumeId, userId);
                return Ok(new { success = true, message = "Resume deleted" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { success = false, message = exception.Message });
            }
        }

        [HttpPost("{resumeId:guid}/restore")]
        public async Task<IActionResult> Restore(Guid resumeId)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.upload")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            try
            {
                await _resumeService.RestoreAsync(resumeId, userId);
                return Ok(new { success = true, message = "Resume restored" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { success = false, message = exception.Message });
            }
        }

        [HttpPost("{resumeId:guid}/versions")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadVersion(
            Guid resumeId,
            [FromForm] ResumeUploadRequest request,
            CancellationToken cancellationToken)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.upload")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            if (request.File == null)
                return BadRequest(new { success = false, message = "Resume file is required" });

            try
            {
                await using var stream = request.File.OpenReadStream();
                var version = await _resumeService.UploadVersionAsync(
                    resumeId,
                    stream,
                    request.File.FileName,
                    request.File.ContentType,
                    userId,
                    cancellationToken);

                return Ok(new { success = true, data = version });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        [HttpGet("{resumeId:guid}/versions")]
        public async Task<IActionResult> Versions(Guid resumeId)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            try
            {
                var versions = await _resumeService.GetVersionsAsync(resumeId, userId);
                return Ok(new { success = true, data = versions });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { success = false, message = exception.Message });
            }
        }

        [HttpGet("{resumeId:guid}/versions/{versionId:guid}")]
        public async Task<IActionResult> Version(Guid resumeId, Guid versionId)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            try
            {
                var version = await _resumeService.GetVersionAsync(resumeId, versionId, userId);
                return version == null
                    ? NotFound(new { success = false, message = "Version not found" })
                    : Ok(new { success = true, data = version });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return NotFound(new { success = false, message = exception.Message });
            }
        }

        [HttpPost("{resumeId:guid}/versions/{versionId:guid}/current")]
        public async Task<IActionResult> SetCurrentVersion(Guid resumeId, Guid versionId)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.resume.upload")) return Forbid();
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            try
            {
                await _resumeService.SetCurrentVersionAsync(resumeId, versionId, userId);
                return Ok(new { success = true, message = "Current version updated" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { success = false, message = exception.Message });
            }
        }

        private bool TryGetUserId(out Guid userId)
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out userId);
        }
    }
}
