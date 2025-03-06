using Xunit;

namespace UserService.Tests.FunctionalTests.Base.Exception.GraphQl;

public class GraphQlExceptionBaseFunctionalTest : IClassFixture<GraphQlExceptionFunctionalTestWebAppFactory>
{
    protected readonly HttpClient HttpClient;

    protected GraphQlExceptionBaseFunctionalTest(GraphQlExceptionFunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }
}