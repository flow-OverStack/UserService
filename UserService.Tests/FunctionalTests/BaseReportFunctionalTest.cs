using Xunit;

namespace UserService.Tests.FunctionalTests;

public class BaseReportFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    public BaseReportFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected HttpClient HttpClient { get; init; }
}