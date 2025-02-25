using Serilog;
using UserService.BackgroundTasks.Interfaces;
using UserService.Domain.Interfaces.Services;

namespace UserService.BackgroundTasks.Jobs;

public class ReputationResetJob(IReputationResetService reputationResetService) : IJob
{
    public async Task Run()
    {
        try
        {
            var result = await reputationResetService.ResetEarnedTodayReputation();
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