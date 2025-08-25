using Hangfire;
using Xunit;

namespace UserService.Tests.FunctionalTests.Base;

public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;
    protected readonly IServiceProvider ServiceProvider;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
        ServiceProvider = factory.Services;

        // Hangfire sometimes fails to work with the database in tests (throws exceptions),
        // so we use in-memory storage to keep tests stable and prevent it from using the DB.
        GlobalConfiguration.Configuration.UseInMemoryStorage();
    }
}