using Serilog;
using UserService.Domain.Interfaces.Service;

namespace UserService.BackgroundJobs.Jobs;

public class SyncUserActivitiesJob(IUserActivityDatabaseService userService, ILogger logger)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var result = await userService.SyncHeartbeatsToDatabaseAsync(cancellationToken);
        if (result.IsSuccess)
            logger.Information("Successfully synced {count} activities to database", result.Data.SyncedHeartbeatsCount);
        else
            logger.Error("Failed to sync activities to database: {message}", result.ErrorMessage);
    }
}