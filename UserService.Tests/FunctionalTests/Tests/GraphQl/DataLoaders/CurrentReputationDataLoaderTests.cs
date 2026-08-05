using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Settings;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class CurrentReputationDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task LoadAsync_ExistingUserId_ReturnsReputation()
    {
        //Arrange
        const long userId = 1;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<CurrentReputationDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(userId);

        //Assert
        Assert.True(result >= BusinessRules.MinReputation);
    }

    [Fact]
    public async Task LoadAsync_NonExistentUserId_ReturnsZero()
    {
        //Arrange
        const long userId = 0;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<CurrentReputationDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(userId);

        //Assert
        Assert.Equal(0, result);
    }
}