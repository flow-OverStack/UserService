using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class GroupRoleUserDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task LoadRequiredAsync_ExistingRoleId_ReturnsSuccess()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupRoleUserDataLoader>();
        const long roleId = 1;

        //Act
        var users = await dataLoader.LoadRequiredAsync(roleId);

        //Assert
        Assert.Equal(2, users.Length); // Role with id 1 has 2 users
    }

    [Fact]
    public async Task LoadRequiredAsync_NonExistentRoleId_ReturnsNoUsers()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupRoleUserDataLoader>();
        const long roleId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(roleId);

        //Assert
        Assert.Empty(result);
    }
}