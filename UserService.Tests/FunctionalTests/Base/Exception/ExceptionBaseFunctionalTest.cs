using Xunit;

namespace UserService.Tests.FunctionalTests.Base.Exception;

public class ExceptionBaseFunctionalTest : IClassFixture<ExceptionFunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;

    protected ExceptionBaseFunctionalTest(ExceptionFunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }
}