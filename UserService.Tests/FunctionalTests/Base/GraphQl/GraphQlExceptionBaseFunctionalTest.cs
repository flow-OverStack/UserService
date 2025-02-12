using Xunit;

namespace UserService.Tests.FunctionalTests.Base.GraphQl;

public class GraphQlExceptionBaseFunctionalTest : IClassFixture<GraphQlExceptionFunctionalTestWebAppFactory>
{
    protected GraphQlExceptionBaseFunctionalTest(GraphQlExceptionFunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected HttpClient HttpClient { get; init; }
}