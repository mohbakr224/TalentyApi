using Application.DTOs.Request;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/interviews")]
    [Authorize(Roles = "CANDIDATE,RECRUITER,HR,COMPANY_ADMIN,ADMIN")]
    public class InterviewsController : ControllerBase
    {
        private readonly IInterviewService _interviewService;
        private readonly IPermissionChecker _permissionChecker;

        public InterviewsController(
            IInterviewService interviewService,
            IPermissionChecker permissionChecker)
        {
            _interviewService = interviewService;
            _permissionChecker = permissionChecker;
        }

        [HttpPost]
        public async Task<IActionResult> Schedule([FromBody] CreateInterviewRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var canSchedule = await _permissionChecker.HasPermissionAsync(User, "interview.schedule");
            if (!canSchedule)
                return Forbid();

            try
            {
                var result = await _interviewService.ScheduleAsync(request, userId);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var interview = await _interviewService.GetByIdAsync(id);
            if (interview == null)
                return NotFound(new { success = false, message = "Interview not found" });

            // Allow if candidate or interviewer or has interview.read
            if (interview.InterviewerId != userId && interview.CandidateId != userId)
            {
                var canRead = await _permissionChecker.HasPermissionAsync(User, "interview.read");
                if (!canRead)
                    return Forbid();
            }

            return Ok(new { success = true, data = interview });
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] InterviewSearchFilter filter, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var canRead = await _permissionChecker.HasPermissionAsync(User, "interview.read");
            if (!canRead && filter.CandidateId != userId && filter.InterviewerId != userId)
            {
                // Default candidate to their own interviews if no permission
                filter.CandidateId = userId;
            }

            var list = await _interviewService.SearchAsync(filter, cancellationToken);
            return Ok(new { success = true, data = list });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateInterviewRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var canManage = await _permissionChecker.HasPermissionAsync(User, "interview.manage");
            if (!canManage)
                return Forbid();

            try
            {
                var result = await _interviewService.UpdateAsync(id, request, userId);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] string? reason = null)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var canManage = await _permissionChecker.HasPermissionAsync(User, "interview.manage");
            if (!canManage)
                return Forbid();

            try
            {
                var result = await _interviewService.CancelAsync(id, userId, reason);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id:guid}/feedback")]
        public async Task<IActionResult> SubmitFeedback([FromRoute] Guid id, [FromBody] SubmitInterviewFeedbackRequest request)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized(new { success = false, message = "Invalid user" });

            var canManage = await _permissionChecker.HasPermissionAsync(User, "interview.manage");
            if (!canManage)
                return Forbid();

            try
            {
                var result = await _interviewService.SubmitFeedbackAsync(id, request, userId);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        private bool TryGetUserId(out Guid userId)
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out userId);
        }
    }
}
