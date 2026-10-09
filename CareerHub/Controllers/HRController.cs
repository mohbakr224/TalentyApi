using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Persistence;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace CareerHub.Controllers
{
    [ApiController]
    [Route("api/v1/hr")]
    [Authorize(Roles = "RECRUITER,HR,COMPANY_ADMIN,ADMIN")]
    public class HRController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IApplicationService _applicationService;
        private readonly ICandidateService _candidateService;
        private readonly IResumeService _resumeService;
        private readonly IInterviewService _interviewService;
        private readonly INotificationService _notificationService;
        private readonly ICompanyService _companyService;
        private readonly IPermissionChecker _permissionChecker;
        private readonly AppDBcontext _dbContext;

        public HRController(
            IJobService jobService,
            IApplicationService applicationService,
            ICandidateService candidateService,
            IResumeService resumeService,
            IInterviewService interviewService,
            INotificationService notificationService,
            ICompanyService companyService,
            IPermissionChecker permissionChecker,
            AppDBcontext dbContext)
        {
            _jobService = jobService;
            _applicationService = applicationService;
            _candidateService = candidateService;
            _resumeService = resumeService;
            _interviewService = interviewService;
            _notificationService = notificationService;
            _companyService = companyService;
            _permissionChecker = permissionChecker;
            _dbContext = dbContext;
        }

        // ==========================================
        // 1. HR COMPANY & DASHBOARD
        // ==========================================

        [HttpGet("company")]
        public async Task<IActionResult> GetMyCompany()
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "company.read")) return Forbid();
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var company = await _companyService.GetCompanyForUserAsync(userId);
            if (company == null)
                return NotFound(new { success = false, message = "No company found for the current HR user" });

            return Ok(new { success = true, data = company });
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] Guid? companyId = null)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "application.read")) return Forbid();
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var comp = await ResolveCompanyAsync(userId, companyId);
            if (comp == null)
                return NotFound(new { success = false, message = "No associated company found" });

            var jobs = await _dbContext.Jobs
                .Where(j => j.CompanyId == comp.Id && !j.IsDeleted)
                .ToListAsync();

            var jobIds = jobs.Select(j => j.Id).ToList();

            var applications = await _dbContext.Applications
                .Where(a => jobIds.Contains(a.JobId))
                .ToListAsync();

            var now = DateTime.UtcNow;
            var upcomingInterviews = await _dbContext.Interviews
                .Where(i => i.CompanyId == comp.Id && !i.IsDeleted && i.Status == "SCHEDULED" && i.ScheduledAt >= now)
                .CountAsync();

            var teamMembers = await _dbContext.CompanyMembers
                .Where(m => m.CompanyId == comp.Id && m.IsActive)
                .CountAsync();

            var dashboard = new HRDashboardResponse
            {
                CompanyId = comp.Id,
                CompanyName = comp.Name,
                TotalJobs = jobs.Count,
                ActiveJobs = jobs.Count(j => string.Equals(j.Status, "PUBLISHED", StringComparison.OrdinalIgnoreCase)),
                DraftJobs = jobs.Count(j => string.Equals(j.Status, "DRAFT", StringComparison.OrdinalIgnoreCase)),
                ClosedJobs = jobs.Count(j => string.Equals(j.Status, "CLOSED", StringComparison.OrdinalIgnoreCase)),
                TotalApplications = applications.Count,
                AppliedCount = applications.Count(a => string.Equals(a.Status, "APPLIED", StringComparison.OrdinalIgnoreCase)),
                ScreeningCount = applications.Count(a => string.Equals(a.Status, "SCREENING", StringComparison.OrdinalIgnoreCase)),
                ShortlistedCount = applications.Count(a => string.Equals(a.Status, "SHORTLISTED", StringComparison.OrdinalIgnoreCase)),
                InterviewCount = applications.Count(a => string.Equals(a.Status, "INTERVIEW", StringComparison.OrdinalIgnoreCase)),
                OfferCount = applications.Count(a => string.Equals(a.Status, "OFFER", StringComparison.OrdinalIgnoreCase)),
                HiredCount = applications.Count(a => string.Equals(a.Status, "HIRED", StringComparison.OrdinalIgnoreCase)),
                RejectedCount = applications.Count(a => string.Equals(a.Status, "REJECTED", StringComparison.OrdinalIgnoreCase)),
                UpcomingInterviewsCount = upcomingInterviews,
                TeamMembersCount = teamMembers
            };

            return Ok(new { success = true, data = dashboard });
        }

        // ==========================================
        // 2. CREATE / EDIT / PUBLISH / CLOSE COMPANY JOBS
        // ==========================================

        [HttpGet("jobs")]
        public async Task<IActionResult> GetJobs([FromQuery] Guid? companyId = null, CancellationToken cancellationToken = default)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "job.read")) return Forbid();
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var comp = await ResolveCompanyAsync(userId, companyId);
            if (comp == null) return NotFound(new { success = false, message = "No company found" });

            var jobs = await _jobService.GetByCompanyIdAsync(comp.Id, cancellationToken);
            return Ok(new { success = true, data = jobs });
        }

        [HttpPost("jobs")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canCreate = await _permissionChecker.HasPermissionAsync(User, "job.create");
            if (!canCreate) return Forbid();

            if (request.CompanyId == Guid.Empty)
            {
                var comp = await ResolveCompanyAsync(userId, null);
                if (comp == null)
                    return BadRequest(new { success = false, message = "Please specify a CompanyId or join a company" });
                request.CompanyId = comp.Id;
            }

            var job = await _jobService.CreateAsync(request);
            return Ok(new { success = true, data = job });
        }

        [HttpGet("jobs/{id:guid}")]
        public async Task<IActionResult> GetJob([FromRoute] Guid id)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "job.read")) return Forbid();
            var job = await _jobService.GetByIdAsync(id);
            if (job == null) return NotFound(new { success = false, message = "Job not found" });
            return Ok(new { success = true, data = job });
        }

        [HttpPut("jobs/{id:guid}")]
        public async Task<IActionResult> UpdateJob([FromRoute] Guid id, [FromBody] UpdateJobRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canUpdate = await _permissionChecker.HasPermissionAsync(User, "job.update");
            if (!canUpdate) return Forbid();

            try
            {
                var updated = await _jobService.UpdateAsync(id, request, userId);
                return Ok(new { success = true, data = updated });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("jobs/{id:guid}/publish")]
        public async Task<IActionResult> PublishJob([FromRoute] Guid id)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canPublish = await _permissionChecker.HasPermissionAsync(User, "job.publish");
            if (!canPublish) return Forbid();

            try
            {
                var updated = await _jobService.PublishAsync(id, userId);
                return Ok(new { success = true, data = updated });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("jobs/{id:guid}/close")]
        public async Task<IActionResult> CloseJob([FromRoute] Guid id)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canClose = await _permissionChecker.HasPermissionAsync(User, "job.close");
            if (!canClose) return Forbid();

            try
            {
                var updated = await _jobService.CloseAsync(id, userId);
                return Ok(new { success = true, data = updated });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 3. VIEW APPLICANTS FOR COMPANY'S JOBS
        // ==========================================

        [HttpGet("jobs/{jobId:guid}/applicants")]
        public async Task<IActionResult> GetJobApplicants(
            [FromRoute] Guid jobId,
            [FromQuery] string? status = null,
            [FromQuery] int limit = 20,
            [FromQuery] int offset = 0,
            CancellationToken cancellationToken = default)
        {
            var canView = await _permissionChecker.HasPermissionAsync(User, "application.read") ||
                          await _permissionChecker.HasPermissionAsync(User, "applications.view");
            if (!canView) return Forbid();

            var applicants = await _applicationService.GetByJobIdAsync(jobId, status, limit, offset, cancellationToken);
            return Ok(new { success = true, data = applicants });
        }

        [HttpGet("applications")]
        public async Task<IActionResult> GetAllCompanyApplications(
            [FromQuery] Guid? companyId = null,
            [FromQuery] string? status = null,
            [FromQuery] int limit = 20,
            [FromQuery] int offset = 0,
            CancellationToken cancellationToken = default)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canView = await _permissionChecker.HasPermissionAsync(User, "application.read") ||
                          await _permissionChecker.HasPermissionAsync(User, "applications.view");
            if (!canView) return Forbid();

            var comp = await ResolveCompanyAsync(userId, companyId);
            if (comp == null) return NotFound(new { success = false, message = "No company found" });

            var list = await _applicationService.GetByCompanyIdAsync(comp.Id, status, limit, offset, cancellationToken);
            return Ok(new { success = true, data = list });
        }

        [HttpGet("applications/{id:guid}")]
        public async Task<IActionResult> GetApplication([FromRoute] Guid id)
        {
            var canView = await _permissionChecker.HasPermissionAsync(User, "application.read") ||
                          await _permissionChecker.HasPermissionAsync(User, "applications.view");
            if (!canView) return Forbid();

            var app = await _applicationService.GetByIdAsync(id);
            if (app == null) return NotFound(new { success = false, message = "Application not found" });

            return Ok(new { success = true, data = app });
        }

        // ==========================================
        // 4. SHORTLIST / REJECT / CHANGE APPLICATION STATUS
        // ==========================================

        [HttpPost("applications/{id:guid}/shortlist")]
        public async Task<IActionResult> ShortlistCandidate([FromRoute] Guid id, [FromBody] string? reason = null)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManage = await _permissionChecker.HasPermissionAsync(User, "application.manage") ||
                            await _permissionChecker.HasPermissionAsync(User, "applications.change");
            if (!canManage) return Forbid();

            try
            {
                var updated = await _applicationService.ShortlistAsync(id, userId, reason);
                return Ok(new { success = true, data = updated });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("applications/{id:guid}/reject")]
        public async Task<IActionResult> RejectCandidate([FromRoute] Guid id, [FromBody] string? reason = null)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManage = await _permissionChecker.HasPermissionAsync(User, "application.manage") ||
                            await _permissionChecker.HasPermissionAsync(User, "applications.change");
            if (!canManage) return Forbid();

            try
            {
                var updated = await _applicationService.RejectAsync(id, userId, reason);
                return Ok(new { success = true, data = updated });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("applications/{id:guid}/status")]
        public async Task<IActionResult> ChangeApplicationStatus(
            [FromRoute] Guid id,
            [FromQuery] string status,
            [FromBody] string? reason = null)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManage = await _permissionChecker.HasPermissionAsync(User, "application.manage") ||
                            await _permissionChecker.HasPermissionAsync(User, "applications.change");
            if (!canManage) return Forbid();

            try
            {
                var updated = await _applicationService.ChangeStatusAsync(id, status, userId, reason);
                return Ok(new { success = true, data = updated });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 5. SEARCH / FILTER CANDIDATES & VIEW PROFILES
        // ==========================================

        [HttpGet("candidates")]
        public async Task<IActionResult> SearchCandidates(
            [FromQuery] CandidateSearchFilter filter,
            CancellationToken cancellationToken = default)
        {
            var canSearch = await _permissionChecker.HasPermissionAsync(User, "candidate.search");
            if (!canSearch) return Forbid();

            var candidates = await _candidateService.SearchCandidatesAsync(filter, cancellationToken);
            return Ok(new { success = true, data = candidates });
        }

        [HttpGet("candidates/{id:guid}")]
        public async Task<IActionResult> GetCandidateProfile([FromRoute] Guid id)
        {
            var canView = await _permissionChecker.HasPermissionAsync(User, "candidate.view");
            if (!canView) return Forbid();

            var profile = await _candidateService.GetCandidateByIdAsync(id);
            if (profile == null)
                return NotFound(new { success = false, message = "Candidate profile not found" });

            return Ok(new { success = true, data = profile });
        }

        // ==========================================
        // 6. REVIEW RESUMES (VIEW, DOWNLOAD, PREVIEW)
        // ==========================================

        [HttpGet("resumes/{resumeId:guid}")]
        public async Task<IActionResult> ReviewResume([FromRoute] Guid resumeId)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canReadResume = await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read");
            if (!canReadResume) return Forbid();

            var resume = await _resumeService.GetByIdForReviewAsync(resumeId, userId);
            if (resume == null)
                return NotFound(new { success = false, message = "Resume not found" });

            return Ok(new { success = true, data = resume });
        }

        [HttpGet("resumes/{resumeId:guid}/download")]
        public async Task<IActionResult> DownloadResume([FromRoute] Guid resumeId, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canReadResume = await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read");
            if (!canReadResume) return Forbid();

            var stream = await _resumeService.DownloadForReviewAsync(resumeId, userId, cancellationToken);
            if (stream == null)
                return NotFound(new { success = false, message = "Resume file not found" });

            return File(stream, "application/pdf", $"resume-{resumeId}.pdf");
        }

        [HttpGet("resumes/{resumeId:guid}/preview")]
        public async Task<IActionResult> PreviewResume([FromRoute] Guid resumeId, CancellationToken cancellationToken)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canReadResume = await _permissionChecker.HasPermissionAsync(User, "candidate.resume.read");
            if (!canReadResume) return Forbid();

            var stream = await _resumeService.DownloadForReviewAsync(resumeId, userId, cancellationToken);
            if (stream == null)
                return NotFound(new { success = false, message = "Resume file not found" });

            return File(stream, "application/pdf");
        }

        // ==========================================
        // 7. SCHEDULE / MANAGE INTERVIEWS
        // ==========================================

        [HttpPost("interviews")]
        public async Task<IActionResult> ScheduleInterview([FromBody] CreateInterviewRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canSchedule = await _permissionChecker.HasPermissionAsync(User, "interview.schedule");
            if (!canSchedule) return Forbid();

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

        [HttpGet("interviews")]
        public async Task<IActionResult> GetInterviews(
            [FromQuery] InterviewSearchFilter filter,
            CancellationToken cancellationToken = default)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canRead = await _permissionChecker.HasPermissionAsync(User, "interview.read");
            if (!canRead) return Forbid();

            if (!filter.CompanyId.HasValue)
            {
                var comp = await ResolveCompanyAsync(userId, null);
                if (comp != null) filter.CompanyId = comp.Id;
            }

            var list = await _interviewService.SearchAsync(filter, cancellationToken);
            return Ok(new { success = true, data = list });
        }

        [HttpGet("interviews/{id:guid}")]
        public async Task<IActionResult> GetInterview([FromRoute] Guid id)
        {
            var canRead = await _permissionChecker.HasPermissionAsync(User, "interview.read");
            if (!canRead) return Forbid();

            var interview = await _interviewService.GetByIdAsync(id);
            if (interview == null)
                return NotFound(new { success = false, message = "Interview not found" });

            return Ok(new { success = true, data = interview });
        }

        [HttpPut("interviews/{id:guid}")]
        public async Task<IActionResult> UpdateInterview([FromRoute] Guid id, [FromBody] UpdateInterviewRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManage = await _permissionChecker.HasPermissionAsync(User, "interview.manage");
            if (!canManage) return Forbid();

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

        [HttpPost("interviews/{id:guid}/cancel")]
        public async Task<IActionResult> CancelInterview([FromRoute] Guid id, [FromBody] string? reason = null)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManage = await _permissionChecker.HasPermissionAsync(User, "interview.manage");
            if (!canManage) return Forbid();

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

        [HttpPost("interviews/{id:guid}/feedback")]
        public async Task<IActionResult> SubmitInterviewFeedback(
            [FromRoute] Guid id,
            [FromBody] SubmitInterviewFeedbackRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManage = await _permissionChecker.HasPermissionAsync(User, "interview.manage");
            if (!canManage) return Forbid();

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

        // ==========================================
        // 8. SEND RECRUITMENT-RELATED NOTIFICATIONS
        // ==========================================

        [HttpPost("notifications")]
        public async Task<IActionResult> SendRecruitmentNotification([FromBody] SendNotificationRequest request)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canSend = await _permissionChecker.HasPermissionAsync(User, "notification.send");
            if (!canSend) return Forbid();

            if (!request.CompanyId.HasValue)
            {
                var comp = await ResolveCompanyAsync(userId, null);
                if (comp != null) request.CompanyId = comp.Id;
            }

            var result = await _notificationService.SendNotificationAsync(request, userId);
            return Ok(new { success = true, data = result });
        }

        // ==========================================
        // 9. MANAGE RECRUITMENT TEAM MEMBERS
        // ==========================================

        [HttpGet("team")]
        public async Task<IActionResult> GetTeamMembers([FromQuery] Guid? companyId = null)
        {
            if (!await _permissionChecker.HasPermissionAsync(User, "company.read")) return Forbid();
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var comp = await ResolveCompanyAsync(userId, companyId);
            if (comp == null) return NotFound(new { success = false, message = "No company found" });

            var members = await _companyService.GetMembersAsync(comp.Id);
            return Ok(new { success = true, data = members });
        }

        [HttpPost("team")]
        public async Task<IActionResult> AddTeamMember(
            [FromBody] AddCompanyMemberRequest request,
            [FromQuery] Guid? companyId = null)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManageMembers = await _permissionChecker.HasPermissionAsync(User, "company.members.manage");
            if (!canManageMembers) return Forbid();

            var comp = await ResolveCompanyAsync(userId, companyId);
            if (comp == null) return NotFound(new { success = false, message = "No company found" });

            try
            {
                var member = await _companyService.AddMemberAsync(comp.Id, request, userId);
                return Ok(new { success = true, data = member });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPatch("team/{targetUserId:guid}")]
        public async Task<IActionResult> UpdateTeamMemberRole(
            [FromRoute] Guid targetUserId,
            [FromQuery] string role,
            [FromQuery] Guid? companyId = null)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManageMembers = await _permissionChecker.HasPermissionAsync(User, "company.members.manage");
            if (!canManageMembers) return Forbid();

            var comp = await ResolveCompanyAsync(userId, companyId);
            if (comp == null) return NotFound(new { success = false, message = "No company found" });

            try
            {
                var member = await _companyService.UpdateMemberRoleAsync(comp.Id, targetUserId, role, userId);
                return Ok(new { success = true, data = member });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("team/{targetUserId:guid}")]
        public async Task<IActionResult> RemoveTeamMember(
            [FromRoute] Guid targetUserId,
            [FromQuery] Guid? companyId = null)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var canManageMembers = await _permissionChecker.HasPermissionAsync(User, "company.members.manage");
            if (!canManageMembers) return Forbid();

            var comp = await ResolveCompanyAsync(userId, companyId);
            if (comp == null) return NotFound(new { success = false, message = "No company found" });

            var success = await _companyService.RemoveMemberAsync(comp.Id, targetUserId, userId);
            if (!success)
                return NotFound(new { success = false, message = "Member not found" });

            return Ok(new { success = true, message = "Team member removed" });
        }

        // ==========================================
        // HELPERS
        // ==========================================

        private bool TryGetUserId(out Guid userId)
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out userId);
        }

        private async Task<CompanyResponse?> ResolveCompanyAsync(Guid userId, Guid? preferredCompanyId)
        {
            if (preferredCompanyId.HasValue)
            {
                return await _companyService.GetByIdAsync(preferredCompanyId.Value);
            }

            return await _companyService.GetCompanyForUserAsync(userId);
        }
    }
}
