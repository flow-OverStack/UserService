using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class RoleDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task LoadAsync_ExistingRoleId_ReturnsSuccess()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<RoleDataLoader>();
        const long roleId = 1;

        //Act
        var result = await dataLoader.LoadAsync(roleId);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task LoadAsync_NonExistentRoleId_ReturnsNull()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<RoleDataLoader>();
        const long roleId = 0;

        //Act
        var result = await dataLoader.LoadAsync(roleId);

        //Assert
        Assert.Null(result);
    }
}