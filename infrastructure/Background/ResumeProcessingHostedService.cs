using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace infrastructure.Background
{
    public sealed class ResumeProcessingHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<ResumeProcessingHostedService> _logger;

        public ResumeProcessingHostedService(
            IBackgroundTaskQueue taskQueue,
            ILogger<ResumeProcessingHostedService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var workItem = await _taskQueue.DequeueAsync(stoppingToken);
                    await workItem(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error occurred while processing a background resume task.");
                }
            }
        }
    }
}
