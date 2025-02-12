using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Resources;
using UserService.Tests.Configurations;
using UserService.Tests.Constants;
using UserService.Tests.Extensions;
using UserService.Tests.FunctionalTests.Base.GraphQl;
using UserService.Tests.FunctionalTests.Configurations.GrpahQl;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class GraphQlExceptionalTests : GraphQlExceptionBaseFunctionalTest
{
    public GraphQlExceptionalTests(GraphQlExceptionFunctionalTestWebAppFactory factory) : base(factory)
    {
        const string serviceName = "testservice1";
        HttpClient.SetGraphQlAuthHeaders(serviceName);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_ServerError()
    {
        //Arrange
        var requestBody = new { query = GraphQlConstants.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlConstants.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        var isServerError = result!.Errors.All(x =>
            x.Message == $"{ErrorMessage.InternalServerError}: {TestException.ErrorMessage}");

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(isServerError);
    }
}