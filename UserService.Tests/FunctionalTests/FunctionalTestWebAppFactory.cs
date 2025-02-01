using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using UserService.DAL;
using UserService.Domain.Settings;
using UserService.Tests.Extensions;
using UserService.Tests.FunctionalTests.Configurations;
using UserService.Tests.FunctionalTests.Configurations.Keycloak;
using Xunit;

namespace UserService.Tests.FunctionalTests;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _keycloakPostgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("keycloak-db")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private readonly PostgreSqlContainer _userServicePostgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("user-service-db")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    public async Task InitializeAsync()
    {
        await _userServicePostgreSql.StartAsync();
        await _keycloakPostgreSql.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _userServicePostgreSql.StopAsync();
        await _keycloakPostgreSql.StopAsync();
        WireMockConfiguration.StopServer();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            var userServiceConnectionString = _userServicePostgreSql.GetConnectionString();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(userServiceConnectionString));

            var keycloakConnectionString = _keycloakPostgreSql.GetConnectionString();
            services.AddDbContext<KeycloakDbContext>(options => options.UseNpgsql(keycloakConnectionString));

            services.RemoveAll<KeycloakSettings>();
            services.Configure<KeycloakSettings>(x =>
            {
                x.Url = "http://localhost:" + WireMockConfiguration.Port;
                x.Realm = WireMockConfiguration.RealmName;
                x.AdminToken = "TestAdminToken";
                x.Audience = TokenExtensions.GetAudience();
                x.ClientId = "TestClientId";
                x.RolesAttributeName = "roles";
                x.UserIdAttributeName = "userId";
            });

            services.PrepPopulation();
            WireMockConfiguration.StartServer(services);
        });
    }
}