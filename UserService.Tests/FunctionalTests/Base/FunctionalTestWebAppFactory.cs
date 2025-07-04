using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using UserService.DAL;
using UserService.Domain.Settings;
using UserService.Tests.FunctionalTests.Configurations;
using UserService.Tests.FunctionalTests.Configurations.Keycloak;
using UserService.Tests.FunctionalTests.Extensions;
using UserService.Tests.FunctionalTests.Helpers;
using WireMock.Server;
using Xunit;

namespace UserService.Tests.FunctionalTests.Base;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _keycloakPostgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("keycloak-db")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:latest")
        .Build();

    private readonly PostgreSqlContainer _userServicePostgreSql = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("user-service-db")
        .WithUsername("postgres")
        .WithPassword("root")
        .Build();

    private WireMockServer _wireMockServer = null!;

    public async Task InitializeAsync()
    {
        await _userServicePostgreSql.StartAsync();
        await _keycloakPostgreSql.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _userServicePostgreSql.StopAsync();
        await _keycloakPostgreSql.StopAsync();
        await _redisContainer.StopAsync();
        _wireMockServer.StopServer();
        _wireMockServer.Dispose();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:PostgresSQL", _userServicePostgreSql.GetConnectionString() }
                }!)
                .Build();

            config.AddConfiguration(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            var userServiceConnectionString = _userServicePostgreSql.GetConnectionString();
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(userServiceConnectionString));

            services.RemoveAll(typeof(DbContextOptions<KeycloakDbContext>));
            var keycloakConnectionString = _keycloakPostgreSql.GetConnectionString();
            services.AddDbContext<KeycloakDbContext>(options => options.UseNpgsql(keycloakConnectionString));

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            scope.PrepPopulation();

            _wireMockServer = _wireMockServer.StartServer(services);

            services.RemoveAll<IOptions<KeycloakSettings>>();
            services.Configure<KeycloakSettings>(x =>
            {
                x.Host = _wireMockServer.Url!;
                x.Realm = WireMockIdentityServerExtensions.RealmName;
                x.AdminToken = "TestAdminToken";
                x.Audience = TokenHelper.GetAudience();
                x.ClientId = "TestClientId";
                x.RolesClaim = "roles";
                x.UserIdClaim = "userId";
            });

            services.RemoveAll<IOptions<RedisSettings>>();
            services.Configure<RedisSettings>(x =>
            {
                _redisContainer.GetConnectionString().ParseConnectionString(out var host, out var port);
                x.Host = host;
                x.Port = port;
                x.Password = null!;
            });
        });
    }
}