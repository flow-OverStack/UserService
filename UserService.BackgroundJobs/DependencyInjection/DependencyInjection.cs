using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundJobs.Jobs;

namespace UserService.BackgroundJobs.DependencyInjection;

public static class DependencyInjection
{
    /// <summary>
    ///     Sets up hangfire jobs
    /// </summary>
    /// <param name="app"></param>
    public static void SetupHangfireJobs(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
            {
                using var scope = app.Services.CreateAsyncScope();
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                recurringJobManager.AddOrUpdate<ReputationResetJob>("ReputationDailyReset",
                    job => job.RunAsync(CancellationToken.None), Cron.Daily);
                recurringJobManager.AddOrUpdate<ProcessedEventsResetJob>("ProcessedEventsReset",
                    job => job.RunAsync(CancellationToken.None), Cron.Daily);
            }
        );
    }
}