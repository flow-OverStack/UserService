using Microsoft.Extensions.Options;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Settings;
using UserService.Keycloak;
using UserService.Tests.UnitTests.Configurations;

namespace UserService.Tests.UnitTests.ServiceFactories;

public class IdentityServerFactory
{
    private readonly IIdentityServer _identityServer;

    public readonly IHttpClientFactory HttpClientFactory =
        HttpClientFactoryConfiguration.GetHttpClientFactoryConfiguration();

    public readonly KeycloakSettings KeycloakSettings = new()
    {
        Url = "testUri:0",
        Realm = "TestRealm",
        AdminToken = "TestAdminToken",
        ClientId = "TestClientId",
        Audience = "TestAudience",
        ServiceAudience = "TestServiceAudience",
        UserIdAttributeName = "TestUserIdAttributeName",
        RolesAttributeName = "TestRoleIdAttributeName"
    };

    public IdentityServerFactory()
    {
        _identityServer =
            new KeycloakServer(new OptionsWrapper<KeycloakSettings>(KeycloakSettings), HttpClientFactory);
    }

    public IIdentityServer GetService()
    {
        return _identityServer;
    }
}