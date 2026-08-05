using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Application.Resources;
using UserService.Tests.FunctionalTests.Base.Exception.GraphQl;
using UserService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

[FunctionalTest]
public class GraphQlExceptionTests(ExceptionGraphQlFunctionalTestWebAppFactory factory)
    : ExceptionGraphQlFunctionalTest(factory)
{
    [Fact]
    public async Task GetAll_RepositoryThrows_ReturnsServerError()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body)!;

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(result.Errors, x => x.Message.StartsWith(ErrorMessage.InternalServerError));
    }
}