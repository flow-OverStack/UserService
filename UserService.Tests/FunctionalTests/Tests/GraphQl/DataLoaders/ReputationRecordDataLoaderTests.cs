using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class ReputationRecordDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadAsync_ExistingReputationRecordId_ReturnsSuccess()
    {
        //Arrange
        const long reputationRecordId = 1;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<ReputationRecordDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(reputationRecordId);

        //Assert
        Assert.NotNull(result);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadAsync_NonExistentReputationRecordId_ReturnsNull()
    {
        //Arrange
        const long reputationRecordId = 0;
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<ReputationRecordDataLoader>();

        //Act
        var result = await dataLoader.LoadAsync(reputationRecordId);

        //Assert
        Assert.Null(result);
    }
}