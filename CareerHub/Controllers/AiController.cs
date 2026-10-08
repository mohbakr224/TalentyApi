using System.Security.Claims;
using System.Text.Json;
using Application.Interfaces.Services;
using Application.Interfaces.Security;
using Domain.Entities.AI;
using infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerHub.Controllers;

[ApiController]
[Route("api/v1/ai")]
[Authorize]
public sealed class AiController : ControllerBase
{
    private static readonly HashSet<string> SupportedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "cv-analysis", "portfolio-analysis", "job-description-analysis", "ai-interview",
        "interview-evaluation", "career-analysis", "matching"
    };

    private readonly AppDBcontext _db;
    private readonly IAiAnalysisService _analysis;
    private readonly IPermissionChecker _permissionChecker;

    public AiController(AppDBcontext db, IAiAnalysisService analysis, IPermissionChecker permissionChecker) { _db = db; _analysis = analysis; _permissionChecker = permissionChecker; }

    [HttpPost("analyze/{type}")]
    public async Task<IActionResult> Analyze(string type, [FromBody] JsonElement input, CancellationToken cancellationToken)
    {
        if (!await _permissionChecker.HasPermissionAsync(User, "ai.use")) return Forbid();
        if (!SupportedTypes.Contains(type)) return BadRequest(new { message = "Unsupported AI analysis type." });
        try { return Ok(new { type, result = await _analysis.AnalyzeAsync(type, input.GetRawText(), cancellationToken) }); }
        catch (InvalidOperationException ex) { return StatusCode(503, new { message = ex.Message }); }
    }

    [HttpPost("jobs/{type}")]
    public async Task<IActionResult> Submit(string type, [FromBody] JsonElement input, CancellationToken cancellationToken)
    {
        if (!await _permissionChecker.HasPermissionAsync(User, "ai.use")) return Forbid();
        if (!SupportedTypes.Contains(type)) return BadRequest(new { message = "Unsupported AI analysis type." });
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var job = new AiAnalysisJob { Id = Guid.NewGuid(), UserId = userId, Type = type, InputJson = input.GetRawText(), Status = "Pending", CreatedDate = DateTime.UtcNow };
        _db.AiAnalysisJobs.Add(job);
        await _db.SaveChangesAsync(cancellationToken);
        return AcceptedAtAction(nameof(GetJob), new { id = job.Id }, new { job.Id, job.Type, job.Status, statusUrl = $"/api/v1/ai/jobs/{job.Id}" });
    }

    [HttpGet("jobs/{id:guid}")]
    public async Task<IActionResult> GetJob(Guid id, CancellationToken cancellationToken)
    {
        if (!await _permissionChecker.HasPermissionAsync(User, "ai.use")) return Forbid();
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return Unauthorized();
        var job = await _db.AiAnalysisJobs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        return job == null ? NotFound() : Ok(new { job.Id, job.Type, job.Status, job.Result, job.Error, job.CreatedDate, job.CompletedAt });
    }
}
