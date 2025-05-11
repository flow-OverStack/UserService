using Xunit;

namespace UserService.Tests.FunctionalTests.Base.Exception.GraphQl;

public class ExceptionGraphQlFunctionalTest : IClassFixture<ExceptionGraphQlFunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;

    protected ExceptionGraphQlFunctionalTest(ExceptionGraphQlFunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }
}