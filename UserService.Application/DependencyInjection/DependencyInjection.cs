using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Mapping;
using UserService.Application.Services;
using UserService.Domain.Interfaces.Services;

namespace UserService.Application.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserMapping));

        InitServices(services);
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRoleService, RoleService>();
    }
}