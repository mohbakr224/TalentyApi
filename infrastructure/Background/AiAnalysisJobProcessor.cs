using Application.Interfaces.Services;
using infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace infrastructure.Background;

public sealed class AiAnalysisJobProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AiAnalysisJobProcessor> _logger;
    public AiAnalysisJobProcessor(IServiceScopeFactory scopeFactory, ILogger<AiAnalysisJobProcessor> logger) { _scopeFactory = scopeFactory; _logger = logger; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var startupScope = _scopeFactory.CreateScope())
        {
            var startupDb = startupScope.ServiceProvider.GetRequiredService<AppDBcontext>();
            await startupDb.AiAnalysisJobs.Where(x => x.Status == "Processing")
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Status, "Pending"), stoppingToken);
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDBcontext>();
                var job = await db.AiAnalysisJobs.Where(x => x.Status == "Pending").OrderBy(x => x.CreatedDate).FirstOrDefaultAsync(stoppingToken);
                if (job == null) { await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken); continue; }
                job.Status = "Processing";
                await db.SaveChangesAsync(stoppingToken);
                try
                {
                    var analysis = scope.ServiceProvider.GetRequiredService<IAiAnalysisService>();
                    job.Result = await analysis.AnalyzeAsync(job.Type, job.InputJson, stoppingToken);
                    job.Status = "Completed";
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    job.Status = "Failed";
                    job.Error = ex.Message;
                    _logger.LogError(ex, "AI analysis job {JobId} failed", job.Id);
                }
                job.CompletedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception ex) { _logger.LogError(ex, "AI job processor failed"); await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); }
        }
    }
}
