using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UserService.BackgroundJobs.Jobs;
using UserService.BackgroundJobs.Queues;
using UserService.Domain.Interfaces.Provider;

namespace UserService.BackgroundJobs.DependencyInjection;

public static class DependencyInjection
{
    /// <summary>
    ///     Registers the Hangfire-backed adapters for the identity-compensation and user-sync ports.
    /// </summary>
    /// <param name="services"></param>
    public static void AddBackgroundQueues(this IServiceCollection services)
    {
        services.AddScoped<IIdentityCompensationQueue, HangfireIdentityCompensationQueue>();
        services.AddScoped<IUserSyncQueue, HangfireUserSyncQueue>();
    }

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
                recurringJobManager.AddOrUpdate<ProcessedEventsResetJob>("ProcessedEventsReset",
                    job => job.RunAsync(CancellationToken.None), Cron.Daily);
                recurringJobManager.AddOrUpdate<SyncUserActivitiesJob>("ActivitiesSynchronization",
                    job => job.RunAsync(CancellationToken.None), "*/10 * * * *"); // Every ten minutes 
            }
        );
    }
}