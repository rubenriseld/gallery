using Quartz;

namespace Gallery.API;

public class PingJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Pinged at " + DateTime.UtcNow);
        return Task.CompletedTask;
    }
}
