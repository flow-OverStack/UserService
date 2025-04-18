using GreenDonut;
using HotChocolate;
using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Resources;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class GroupUserDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadGroupedBatch_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserDataLoader>();
        var roleIds = new List<long> { 1, 2 };

        //Act
        var users = await dataLoader.LoadRequiredAsync(roleIds);

        //Assert
        Assert.Equal(users.Count, roleIds.Count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_RolesNotFound()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserDataLoader>();
        var roleIds = new List<long> { 0 };

        //Act
        var action = async () => await dataLoader.LoadRequiredAsync(roleIds);

        //Assert
        var exception = await Assert.ThrowsAsync<GraphQLException>(action);
        Assert.Equal(ErrorMessage.UsersNotFound, exception.Message);
    }
}