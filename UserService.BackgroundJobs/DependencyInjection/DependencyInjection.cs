using Hangfire;
using Microsoft.AspNetCore.Builder;
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
                RecurringJob.AddOrUpdate<ReputationResetJob>("ReputationDailyReset",
                    job => job.RunAsync(CancellationToken.None), Cron.Daily);
                RecurringJob.AddOrUpdate<ProcessedEventsResetJob>("ProcessedEventsReset",
                    job => job.RunAsync(CancellationToken.None), Cron.Daily);
            }
        );
    }
}