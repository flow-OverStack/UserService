using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using UserService.Cache.Providers;
using UserService.Cache.Repositories;
using UserService.Cache.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using Role = UserService.Domain.Entities.Role;

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
                Password = redisSettings.Password
            };

            return ConnectionMultiplexer.Connect(configuration);
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
        services.AddScoped<IBaseCacheRepository<User, long>, UserCacheRepository>();
        services.AddScoped<IBaseCacheRepository<Role, long>, RoleCacheRepository>();
    }
}