using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Resources;
using UserService.Tests.FunctionalTests.Base.Exception.GraphQl;
using UserService.Tests.FunctionalTests.Configurations.GraphQl;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class GraphQlExceptionTests(ExceptionGraphQlFunctionalTestWebAppFactory factory)
    : ExceptionGraphQlFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_ServerError()
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