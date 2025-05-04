using Serilog;
using UserService.Domain.Interfaces.Service;

namespace UserService.BackgroundJobs.Jobs;

public class ProcessedEventsResetJob(IProcessedEventsResetService eventsResetService, ILogger logger)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await eventsResetService.ResetProcessedEventsAsync(cancellationToken);
            if (result.IsSuccess)
                logger.Information("Successfully reset processed events");
            else
                logger.Error("Failed to reset processed events: {message}", result.ErrorMessage);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to reset processed events: {message}", e.Message);
        }
    }
}