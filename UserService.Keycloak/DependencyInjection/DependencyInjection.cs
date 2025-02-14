using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Interfaces.Services;

namespace UserService.Keycloak.DependencyInjection;

public static class DependencyInjection
{
    public static void AddIdentityServer(this IServiceCollection services)
    {
        services.AddHttpClient();
        InitServices(services);
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityServer, KeycloakServer>();
    }
}