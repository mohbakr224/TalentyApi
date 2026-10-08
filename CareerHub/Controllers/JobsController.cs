using Application.DTOs.Request;
using Application.Interfaces.Services;
using Application.Interfaces.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly Application.Interfaces.Services.IApplicationService _applicationService;
        private readonly IPermissionChecker _permissionChecker;

        public JobsController(IJobService jobService, Application.Interfaces.Services.IApplicationService applicationService, IPermissionChecker permissionChecker)
        {
            _jobService = jobService;
            _applicationService = applicationService;
            _permissionChecker = permissionChecker;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateJobRequest request)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "job.create")) return Forbid();
            var job = await _jobService.CreateAsync(request);
            return Ok(new { success = true, data = job });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var job = await _jobService.GetByIdAsync(id);
            if (job == null) return NotFound(new { success = false, message = "Job not found" });
            return Ok(new { success = true, data = job });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string? title, [FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            var list = await _jobService.SearchAsync(title, limit, offset, HttpContext.RequestAborted);
            return Ok(new { success = true, data = list });
        }

        // Candidate applies to a job. Body may contain optional ResumeId and CoverLetter.
        [HttpPost("{id}/apply")]
        [Authorize]
        public async Task<IActionResult> Apply([FromRoute] Guid id, [FromBody] Application.DTOs.Request.CreateApplicationRequest? request)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.application.create")) return Forbid();
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var createReq = request ?? new Application.DTOs.Request.CreateApplicationRequest();
            createReq.JobId = id;

            try
            {
                var app = await _applicationService.CreateAsync(createReq, userId);
                return Ok(new { success = true, data = app });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
