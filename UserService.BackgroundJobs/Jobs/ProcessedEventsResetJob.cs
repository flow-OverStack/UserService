using Serilog;
using UserService.Domain.Interfaces.Service;

namespace UserService.BackgroundJobs.Jobs;

public class ProcessedEventsResetJob(IProcessedEventsResetService eventsResetService)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await eventsResetService.ResetProcessedEventsAsync(cancellationToken);
            if (result.IsSuccess)
                Log.Information("Successfully reset processed events");
            else
                Log.Error("Failed to reset processed events: {message}", result.ErrorMessage);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to reset processed events: {message}", e.Message);
        }
    }
}