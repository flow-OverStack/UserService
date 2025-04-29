using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UserService.DAL;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Resources;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Configurations.GraphQl;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

[Collection("GraphQlSequentialTests")]
public class GraphQlSequentialTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAllRoles_ShouldBe_NotFoundError()
    {
        //Arrange
        await DeleteRolesAsync();
        var requestBody = new { query = GraphQlHelper.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        var areRolesNotFound = result!.Errors.Exists(x => x.Message == ErrorMessage.RolesNotFound);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(areRolesNotFound);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetById_ShouldBe_RolesNotfound()
    {
        //Arrange
        await AddUserWithNoRolesAsync();
        const long userId = 4;
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(userId) };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        var areRolesNotFound = result!.Errors.Exists(x => x.Message == ErrorMessage.RolesNotFound);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(areRolesNotFound);
    }

    private async Task DeleteRolesAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Set<Role>().ExecuteDeleteAsync();

        await dbContext.SaveChangesAsync();
    }

    private async Task AddUserWithNoRolesAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<User>>();

        await userRepository.CreateAsync(new User
        {
            Username = "testuser4",
            Email = "TestUser4@test.com",
            KeycloakId = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            LastLoginAt = DateTime.UtcNow,
            Reputation = 1,
            ReputationEarnedToday = 0
        });

        await userRepository.SaveChangesAsync();
    }
}