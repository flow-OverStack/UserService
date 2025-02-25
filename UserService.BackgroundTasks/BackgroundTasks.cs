using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UserService.Domain.Interfaces.Services;

namespace UserService.BackgroundTasks;

public static class BackgroundTasks
{
    public static async Task ResetReputation(this IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var reputationResetService = scope.ServiceProvider.GetRequiredService<IReputationResetService>();

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