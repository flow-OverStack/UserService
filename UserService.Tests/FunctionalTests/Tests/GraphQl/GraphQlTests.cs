using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Resources;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Configurations.GraphQl;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class GraphQlTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetAllResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Users);
        Assert.NotNull(result.Data.Roles);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUserById_ShouldBe_Success()
    {
        //Arrange
        const long userId = 1;
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(userId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetUserByIdResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.User);
        Assert.NotNull(result.Data.User.Roles);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUserById_ShouldBe_UserNotFound()
    {
        //Arrange
        const long userId = 0;
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(userId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(result!.Errors, x => x.Message == ErrorMessage.UserNotFound);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetRoleById_ShouldBe_Success()
    {
        //Arrange
        const long roleId = 1;
        var requestBody = new { query = GraphQlHelper.RequestRoleByIdQuery(roleId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetRoleByIdResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Role);
        Assert.NotNull(result.Data.Role.Users);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetRoleById_ShouldBe_UserNotFound()
    {
        //Arrange
        const long userId = 0;
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(userId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(result!.Errors, x => x.Message == ErrorMessage.UserNotFound);
    }
}