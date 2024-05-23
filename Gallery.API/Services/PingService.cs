using Microsoft.Extensions.Options;
using Quartz;

namespace Gallery.API.Services;

public class PingService : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var pingJobKey = JobKey.Create(nameof(PingJob));
        options.AddJob<PingJob>(jobBuilder => jobBuilder.WithIdentity(pingJobKey))
            .AddTrigger(trigger =>
            trigger.WithSimpleSchedule(schedule =>
                schedule.WithIntervalInSeconds(10)
                    .RepeatForever())
                   .ForJob(pingJobKey));
    }
}
