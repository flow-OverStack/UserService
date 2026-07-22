using AutoMapper;
using Microsoft.Extensions.Options;
using UserService.Domain.Interfaces.Identity;
using UserService.Keycloak;
using UserService.Keycloak.Settings;
using UserService.Tests.UnitTests.Fixtures;

namespace UserService.Tests.UnitTests.Sut;

internal class ExceptionIdentityServerSut
{
    private readonly IIdentityServer _identityServer;

    public readonly HttpClient HttpClient =
        ExceptionHttpClientFixture.GetHttpClientConfiguration("http://testkeycloakserver:8080");

    public readonly KeycloakSettings KeycloakSettings = new()
    {
        Host = "http://testkeycloakserver:8080",
        Realm = "TestRealm",
        AdminToken = "TestAdminToken",
        ClientId = "TestClientId",
        Audience = "TestAudience",
        UserIdClaim = "TestUserIdAttributeName",
        RolesClaim = "TestRoleIdAttributeName"
    };

    public readonly IMapper Mapper = MapperFixture.GetMapperConfiguration();

    public ExceptionIdentityServerSut()
    {
        _identityServer =
            new KeycloakExceptionDecorator(new KeycloakServer(Options.Create(KeycloakSettings), HttpClient, Mapper));
    }

    public IIdentityServer GetService()
    {
        return _identityServer;
    }
}
