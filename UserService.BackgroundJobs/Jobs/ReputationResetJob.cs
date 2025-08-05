using Serilog;
using UserService.Domain.Interfaces.Service;

namespace UserService.BackgroundJobs.Jobs;

public class ReputationResetJob(IReputationResetService reputationResetService, ILogger logger)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var result = await reputationResetService.ResetEarnedTodayReputationAsync(cancellationToken);
        if (result.IsSuccess)
            logger.Information("Successfully reset reputation");
        else
            logger.Error("Failed to reset reputation: {message}", result.ErrorMessage);
    }
}