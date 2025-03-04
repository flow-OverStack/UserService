using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Tests.Constants;
using UserService.Tests.Extensions;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Configurations.GrpahQl;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class GraphQlTests : BaseFunctionalTest
{
    private const string NotAuthenticatedCode = "AUTH_NOT_AUTHENTICATED";

    public GraphQlTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        const string serviceName = "testservice1";
        HttpClient.SetGraphQlAuthHeaders(serviceName);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = GraphQlConstants.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlConstants.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Users);
        Assert.NotNull(result.Data.Roles);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_NotAuthenticated()
    {
        //Arrange
        const string username = "testservice1";
        var userToken = TokenExtensions.GetRsaTokenWithWrongAudience(username);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var requestBody = new { query = GraphQlConstants.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlConstants.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);
        var isNotAuthorized = result!.Errors.All(x => x.Extensions.Code == NotAuthenticatedCode);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(isNotAuthorized);
    }
}