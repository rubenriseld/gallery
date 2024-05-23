using Quartz;

namespace Gallery.API;

public class PingJob : IJob
{
    private readonly ILogger<PingJob> _logger;
    public PingJob(ILogger<PingJob> logger)
    {
        _logger = logger;
    }
    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Pinged at " + DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
