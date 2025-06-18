using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using UserService.Application.Mappings;
using UserService.Application.Services;
using UserService.Application.Services.Cache;
using UserService.Application.Validators;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Interfaces.Validation;
using UserService.Domain.Settings;

namespace UserService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserMapping));
        services.InitRedisCaching();
        services.InitServices();
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<GetUserService>();
        services.AddScoped<IGetUserService, CacheGetUserService>();
        services.AddScoped<GetRoleService>();
        services.AddScoped<IGetRoleService, CacheGetRoleService>();
        services.AddScoped<IReputationService, ReputationService>();
        services.AddScoped<IReputationResetService, ReputationService>();
        services.AddScoped<IProcessedEventsResetService, ProcessedEventsResetService>();

        services.AddScoped<INullSafeValidator<OffsetPageDto>, OffsetPageDtoValidator>();
    }

    private static void InitRedisCaching(this IServiceCollection services)
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
    }
}