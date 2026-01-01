using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.Configurations;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class CurrentReputationDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
    {
        //Arrange
        const long userId = 1;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<CurrentReputationDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(userId);

        //Assert
        Assert.True(result >= MockRepositoriesGetters.MinReputation);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Zero()
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