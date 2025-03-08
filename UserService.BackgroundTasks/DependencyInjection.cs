using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserService.BackgroundTasks.Jobs;

namespace UserService.BackgroundTasks;

public static class DependencyInjection
{
    /// <summary>
    ///     Sets up background tasks
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(x => x.UsePostgreSqlStorage(options =>
            {
                var connectionString = configuration.GetConnectionString("PostgresSQL");
                options.UseNpgsqlConnection(connectionString);
            })
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSerilogLogProvider()
            .UseFilter(new AutomaticRetryAttribute
            {
                Attempts = 10,
                DelaysInSeconds = [30, 60, 300, 600, 1800, 43200, 86400] //30sec, 1min, 5min, 10min, 1h, 12h, 24h
            }));

        services.AddHangfireServer();
        services.InitJobs();
    }

    /// <summary>
    ///     Uses hangfire
    /// </summary>
    /// <param name="app"></param>
    public static void SetupHangfire(this WebApplication app)
    {
        app.Lifetime.ApplicationStarted.Register(() =>
            RecurringJob.AddOrUpdate<ReputationResetJob>("ReputationDailyReset", job => job.Run(), Cron.Daily));

        if (app.Environment.IsDevelopment())
            app.UseHangfireDashboard();
    }

    private static void InitJobs(this IServiceCollection services)
    {
        services.AddTransient<ReputationResetJob>();
    }
}