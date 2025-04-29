using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.BackgroundJobs.Jobs;

namespace UserService.BackgroundJobs.DependencyInjection;

public static class DependencyInjection
{
    /// <summary>
    ///     Uses hangfire
    /// </summary>
    /// <param name="app"></param>
    public static void SetupHangfire(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
            {
                RecurringJob.AddOrUpdate<ReputationResetJob>("ReputationDailyReset",
                    job => job.RunAsync(CancellationToken.None), Cron.Daily);
                RecurringJob.AddOrUpdate<ProcessedEventsResetJob>("ProcessedEventsReset",
                    job => job.RunAsync(CancellationToken.None), Cron.Daily);
            }
        );

        if (app.Environment.IsDevelopment())
            app.UseHangfireDashboard();
    }


    /// <summary>
    ///     Initializes jobs
    /// </summary>
    /// <param name="services"></param>
    public static void InitJobs(this IServiceCollection services)
    {
        services.AddTransient<ReputationResetJob>();
        services.AddTransient<ProcessedEventsResetJob>();
    }
}