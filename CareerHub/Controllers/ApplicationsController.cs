using Application.DTOs.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly Application.Interfaces.Security.IPermissionChecker _permissionChecker;

        public ApplicationsController(IApplicationService applicationService, Application.Interfaces.Security.IPermissionChecker permissionChecker)
        {
            _applicationService = applicationService;
            _permissionChecker = permissionChecker;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateApplicationRequest request)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "candidate.application.create"))
                return Forbid();
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var app = await _applicationService.CreateAsync(request, userId);
            return Ok(new { success = true, data = app });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> MyApplications([FromQuery] int limit = 20, [FromQuery] int offset = 0)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var list = await _applicationService.GetByCandidateIdAsync(userId, HttpContext.RequestAborted);
            return Ok(new { success = true, data = list });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var app = await _applicationService.GetByIdAsync(id);
            if (app == null) return NotFound(new { success = false, message = "Application not found" });

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdClaim, out var userId);

            // Allow if owner (candidate) or has permission to view applications
            if (app.CandidateId == userId)
                return Ok(new { success = true, data = app });

            var hasPerm = await _permissionChecker.HasPermissionAsync(User, "applications.view");
            if (hasPerm)
                return Ok(new { success = true, data = app });

            return Forbid();
        }

        [HttpPost("{id}/status")]
        [Authorize]
        public async Task<IActionResult> ChangeStatus([FromRoute] Guid id, [FromQuery] string status, [FromBody] string? reason = null)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var canChange = await _permissionChecker.HasPermissionAsync(User, "applications.change");
            if (!canChange)
                return Forbid();

            var updated = await _applicationService.ChangeStatusAsync(id, status, userId, reason);
            return Ok(new { success = true, data = updated });
        }
    }
}
