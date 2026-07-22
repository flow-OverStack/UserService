using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class UserDataLoadersTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task LoadAsync_ExistingUserId_ReturnsSuccess()
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

    [Fact]
    public async Task LoadAsync_NonExistentUserId_ReturnsNull()
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