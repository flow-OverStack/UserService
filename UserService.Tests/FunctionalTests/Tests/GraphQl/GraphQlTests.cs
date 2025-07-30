using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Application.Resources;
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
        Assert.NotEmpty(result!.Data.Users.Items);
        Assert.NotEmpty(result.Data.Roles.Items);
        Assert.Equal(3, result.Data.Users.TotalCount);
        Assert.Equal(3, result.Data.Roles.TotalCount);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllUsers_ShouldBe_InvalidPaginationError()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestUsersWithInvalidPaginationQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(result!.Errors);
        Assert.StartsWith(ErrorMessage.InvalidPagination, result.Errors[0].Message);
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
    public async Task GetUserById_ShouldBe_Null()
    {
        //Arrange
        const long userId = 0;
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(userId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetUserByIdResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.User);
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
    public async Task GetRoleById_ShouldBe_Null()
    {
        //Arrange
        const long roleId = 0;
        var requestBody = new { query = GraphQlHelper.RequestRoleByIdQuery(roleId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetRoleByIdResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.Role);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestWithWrongArgument_ShouldBe_Error()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestWithWrongArgument };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Single(result!.Errors);
        Assert.NotNull(result.Errors[0].Extensions?.Code);
    }
}