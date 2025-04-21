using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class RoleDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<RoleDataLoader>();
        const long roleId = 1;

        //Act
        var result = await dataLoader.LoadAsync(roleId);

        //Assert
        Assert.NotNull(result);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_Null()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<RoleDataLoader>();
        const long roleId = 0;

        //Act
        var result = await dataLoader.LoadAsync(roleId);

        //Assert
        Assert.Null(result);
    }
}