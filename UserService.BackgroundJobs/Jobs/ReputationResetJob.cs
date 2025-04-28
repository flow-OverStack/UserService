using Serilog;
using UserService.Domain.Interfaces.Services;

namespace UserService.BackgroundJobs.Jobs;

public class ReputationResetJob(IReputationResetService reputationResetService)
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await reputationResetService.ResetEarnedTodayReputationAsync(cancellationToken);
            if (result.IsSuccess)
                Log.Information("Successfully reset reputation");
            else
                Log.Error("Failed to reset reputation: {message}", result.ErrorMessage);
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to reset reputation: {message}", e.Message);
        }
    }
}