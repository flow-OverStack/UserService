using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupUserDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadGrouped_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserDataLoader>();
        const long roleId = 1;

        //Act
        var users = await dataLoader.LoadRequiredAsync(roleId);

        //Assert
        Assert.Equal(2, users.Length); // Role with id 1 has 2 users
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_NoUsers()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupUserDataLoader>();
        const long roleId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(roleId);

        //Assert
        Assert.Empty(result);
    }
}