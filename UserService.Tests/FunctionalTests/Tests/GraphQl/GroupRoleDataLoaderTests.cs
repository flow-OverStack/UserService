using GreenDonut;
using HotChocolate;
using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Resources;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class GroupRoleDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadGroupedBatch_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupRoleDataLoader>();
        var userIds = new List<long> { 1, 2 };

        //Act
        var users = await dataLoader.LoadRequiredAsync(userIds);

        //Assert
        Assert.Equal(users.Count, userIds.Count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_RolesNotFound()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupRoleDataLoader>();
        var userIds = new List<long> { 0 };

        //Act
        var action = async () => await dataLoader.LoadRequiredAsync(userIds);

        //Assert
        var exception = await Assert.ThrowsAsync<GraphQLException>(action);
        Assert.Equal(ErrorMessage.RolesNotFound, exception.Message);
    }
}