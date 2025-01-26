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
using Xunit;

namespace UserService.Tests.FunctionalTests;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("Diary")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        WireMockConfiguration.StartServer();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.StopAsync();
        WireMockConfiguration.StopServer();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            var connectionString = _postgreSqlContainer.GetConnectionString();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            services.RemoveAll<KeycloakSettings>();
            services.Configure<KeycloakSettings>(x =>
            {
                x.Url = "http://localhost:" + WireMockConfiguration.Port;
                x.Realm = WireMockConfiguration.RealmName;
                x.AdminToken = "TestAdminToken";
                x.Audience = "TestAudience";
                x.ClientId = "TestClientId";
                x.RolesAttributeName = "roles";
                x.UserIdAttributeName = "userId";
            });

            services.PrepPopulation();
        });
    }
}