using Microsoft.Extensions.Options;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Settings;
using UserService.Keycloak;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.Factories;

internal class ExceptionIdentityServerFactory
{
    private readonly IIdentityServer _identityServer;

    public readonly IHttpClientFactory HttpClientFactory =
        ExceptionHttpClientFactoryConfiguration.GetHttpClientFactoryConfiguration();

    public readonly KeycloakSettings KeycloakSettings = new()
    {
        Host = "testUri:0",
        Realm = "TestRealm",
        AdminToken = "TestAdminToken",
        ClientId = "TestClientId",
        Audience = "TestAudience",
        UserIdClaim = "TestUserIdAttributeName",
        RolesClaim = "TestRoleIdAttributeName"
    };

    public ExceptionIdentityServerFactory()
    {
        _identityServer =
            new KeycloakServer(new OptionsWrapper<KeycloakSettings>(KeycloakSettings), HttpClientFactory);
    }

    public IIdentityServer GetService()
    {
        return _identityServer;
    }
}