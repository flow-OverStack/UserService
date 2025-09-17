using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Configurations.GraphQl;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class CacheGetServicesTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    // Only functional tests are provided for cache services' success scenarios.
    // This is because cache data mirrors the database, and manually copying test DB data into multiple cache keys/values is impractical and confusing.
    // In functional tests, data is automatically copied from the DB to the cache as needed, following all key/value rules.

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionById_ShouldBe_Ok()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(1) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // 2nd request fetches data from cache
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
    public async Task GetQuestionByIdWithWrongEntryInCache_ShouldBe_Ok()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDatabase>();
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(1) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // Simulate a wrong entry in the cache
        await cache.StringSetAsync("role:1", "Wrong data");
        // 2nd request fetches data from the cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetUserByIdResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.User);
        Assert.NotNull(result.Data.User.Roles);
    }
}