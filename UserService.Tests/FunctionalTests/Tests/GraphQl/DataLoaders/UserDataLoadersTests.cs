using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class UserDataLoadersTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<UserDataLoader>();
        const long userId = 1;

        //Act
        var result = await dataLoader.LoadAsync(userId);

        //Assert
        Assert.NotNull(result);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Null()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<UserDataLoader>();
        const long userId = 0;

        //Act
        var result = await dataLoader.LoadAsync(userId);

        //Assert
        Assert.Null(result);
    }
}