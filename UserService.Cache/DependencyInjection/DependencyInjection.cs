using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;
using UserService.Cache.Interfaces;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Cache.Settings;

namespace UserService.Cache.DependencyInjection;

public static class DependencyInjection
{
    public static void AddCache(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisSettings = provider.GetRequiredService<IOptions<RedisSettings>>().Value;
            var configuration = new ConfigurationOptions
            {
                EndPoints = { { redisSettings.Host, redisSettings.Port } },
                Password = redisSettings.Password,
                AbortOnConnectFail = false
            };

            var multiplexer = ConnectionMultiplexer.Connect(configuration);
            multiplexer.LogConnectionState(provider.GetRequiredService<ILogger>());

            return multiplexer;
        });

        services.AddScoped<IDatabase>(provider =>
        {
            var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return multiplexer.GetDatabase();
        });

        services.InitProviders();
        services.InitRepositories();
    }

    private static void InitProviders(this IServiceCollection services)
    {
        services.AddScoped<ICacheProvider, RedisCacheProvider>();
    }

    private static void InitRepositories(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<UserCacheRepository>()
            .AddClasses(c => c.InExactNamespaceOf<UserCacheRepository>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    private static void LogConnectionState(this IConnectionMultiplexer multiplexer, ILogger logger)
    {
        var isDown = 0;

        if (!multiplexer.IsConnected)
        {
            isDown = 1;
            logger.Error("Redis at {EndPoints} is unavailable at startup, the cache is bypassed until it connects",
                multiplexer.GetEndPoints());
        }

        multiplexer.ConnectionFailed += (_, e) =>
        {
            if (Interlocked.Exchange(ref isDown, 1) == 0)
                logger.Error(e.Exception,
                    "Redis connection to {EndPoint} failed ({FailureType}), the cache is bypassed",
                    e.EndPoint, e.FailureType);
        };

        multiplexer.ConnectionRestored += (_, e) =>
        {
            if (Interlocked.Exchange(ref isDown, 0) == 1)
                logger.Information("Redis connection to {EndPoint} restored", e.EndPoint);
        };
    }
}