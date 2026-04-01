using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Application.Resources;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
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
        Assert.NotEmpty(result.Data.ReputationRecords.Edges);
        Assert.NotEmpty(result.Data.ReputationRules.Items);
        Assert.Equal(4, result.Data.Users.TotalCount);
        Assert.Equal(3, result.Data.Roles.TotalCount);
        Assert.Equal(9, result.Data.ReputationRecords.TotalCount);
        Assert.Equal(8, result.Data.ReputationRules.TotalCount);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_InvalidPaginationError()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestUsersWithInvalidPaginationQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(4, result!.Errors.Count);
        Assert.All(result.Errors, x => Assert.StartsWith(ErrorMessage.InvalidPagination, x.Message));
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIds_ShouldBe_Success()
    {
        //Arrange
        const long userId = 1, roleId = 1, reputationRuleId = 1, reputationRecordId = 1;
        var requestBody = new
            { query = GraphQlHelper.RequestAllByIdsQuery(userId, roleId, reputationRecordId, reputationRuleId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetAllByIdsResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.User);
        Assert.NotNull(result.Data.Role);
        Assert.NotNull(result.Data.ReputationRule);
        Assert.NotNull(result.Data.ReputationRecord);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetMe_ShouldBe_Success()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken(1, "testuser1");
        var request = new HttpRequestMessage(HttpMethod.Post, GraphQlHelper.GraphQlEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { query = GraphQlHelper.RequestMeQuery });

        //Act
        var response = await HttpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetMeResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Me);
        Assert.Equal(1, result.Data.Me.Id);
        Assert.Equal("testuser1", result.Data.Me.Username);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetMe_ShouldBe_Unauthorized()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestMeQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(result!.Errors);
        Assert.Contains(result.Errors, x => x.Extensions?.Code == "AUTH_NOT_AUTHENTICATED");
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetMe_ShouldBe_Null()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken(0, "testuser1");
        var request = new HttpRequestMessage(HttpMethod.Post, GraphQlHelper.GraphQlEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { query = GraphQlHelper.RequestMeQuery });

        //Act
        var response = await HttpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetMeResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.Me);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllByIds_ShouldBe_Null()
    {
        //Arrange
        const long userId = 0, roleId = 0, reputationRuleId = 0, reputationRecordId = 0;
        var requestBody = new
            { query = GraphQlHelper.RequestAllByIdsQuery(userId, roleId, reputationRecordId, reputationRuleId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetAllByIdsResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.User);
        Assert.Null(result.Data.Role);
        Assert.Null(result.Data.ReputationRule);
        Assert.Null(result.Data.ReputationRecord);
    }
}