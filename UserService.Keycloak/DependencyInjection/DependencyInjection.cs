using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Domain.Interfaces.Identity;
using UserService.Keycloak.Settings;

namespace UserService.Keycloak.DependencyInjection;

public static class DependencyInjection
{
    public static void AddIdentityServer(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(KeycloakServer), (provider, client) =>
        {
            var keycloakSettings = provider.GetRequiredService<IOptions<KeycloakSettings>>().Value;
            client.BaseAddress = new Uri(keycloakSettings.Host);
        });
        InitServices(services);
    }

    private static void InitServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityServer, KeycloakServer>();
    }
}