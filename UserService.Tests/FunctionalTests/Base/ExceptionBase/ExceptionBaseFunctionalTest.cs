using Xunit;

namespace UserService.Tests.FunctionalTests.Base.ExceptionBase;

public class ExceptionBaseFunctionalTest : IClassFixture<ExceptionFunctionalTestWebAppFactory>
{
    protected ExceptionBaseFunctionalTest(ExceptionFunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected HttpClient HttpClient { get; init; }
}