using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Domain.Interfaces.Identity;
using UserService.Keycloak.Mappings;
using UserService.Keycloak.Settings;

namespace UserService.Keycloak.DependencyInjection;

public static class DependencyInjection
{
    public static void AddIdentityServer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(KeycloakUserMapping));

        services.AddHttpClient(KeycloakAuthHandler.TokenClientName, (provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<KeycloakSettings>>().Value;
            client.BaseAddress = new Uri(settings.Host);
        });

        services.AddTransient<KeycloakAuthHandler>();

        services.AddHttpClient<IIdentityServer, KeycloakServer>((provider, client) =>
            {
                var keycloakSettings = provider.GetRequiredService<IOptions<KeycloakSettings>>().Value;
                client.BaseAddress = new Uri(keycloakSettings.Host);
            })
            .AddHttpMessageHandler<KeycloakAuthHandler>();

        services.Decorate<IIdentityServer, KeycloakExceptionDecorator>();
    }
}