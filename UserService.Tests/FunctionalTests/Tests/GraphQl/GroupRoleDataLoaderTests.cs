using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
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
        const long userId = 1;

        //Act
        var roles = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Equal(2, roles.Length); // User with id 1 has 2 roles
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_NoRoles()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupRoleDataLoader>();
        const long userId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Equal(0, result.Length);
    }
}