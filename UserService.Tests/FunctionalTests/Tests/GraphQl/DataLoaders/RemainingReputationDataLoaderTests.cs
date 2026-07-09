using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Settings;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class RemainingReputationDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadAsync_ExistingUserId_ReturnsSuccess()
    {
        //Arrange
        const long userId = 1;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<RemainingReputationDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(userId);

        //Assert
        Assert.InRange(result, 0, BusinessRules.MaxDailyReputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadAsync_NonExistentUserId_ReturnsZero()
    {
        //Arrange
        const long userId = 0;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<RemainingReputationDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(userId);

        //Assert
        Assert.Equal(0, result);
    }
}