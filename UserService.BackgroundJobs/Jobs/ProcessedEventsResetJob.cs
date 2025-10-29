using Serilog;
using UserService.Messaging.Interfaces;

namespace UserService.BackgroundJobs.Jobs;

public class ProcessedEventsResetJob(IProcessedEventsResetService eventsResetService, ILogger logger)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var result = await eventsResetService.ResetProcessedEventsAsync(cancellationToken);
        if (result.IsSuccess)
            logger.Information("Successfully reset processed events");
        else
            logger.Error("Failed to reset processed events: {message}", result.ErrorMessage);
    }
}