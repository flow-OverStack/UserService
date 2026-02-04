using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
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
    public async Task GetUserById_ShouldBe_Ok()
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
    public async Task GetUserById_ShouldBe_Null()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(0) };

        //Act
        // The first request marks the user as null in the cache
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // The second request fetches the null entry from the cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetUserByIdResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(result!.Data.User);
    }


    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetUserById_ShouldBe_Ok_With_WrongEntryInCache()
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetGroupedById_ShouldBe_Null()
    {
        //Arrange
        const long userId = 0;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRoleCacheRepository>();

        //Act
        // The first call marks the user as null in the cache
        await repository.GetUsersRolesOrFetchAndCacheAsync([userId]);
        // The second call fetches the null entry from the cache
        var result = await repository.GetUsersRolesOrFetchAndCacheAsync([userId]);

        //Assert
        Assert.Empty(result);
    }
}