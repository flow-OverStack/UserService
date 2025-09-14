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
    }
}