using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class ReputationRecordDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_Success()
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
    public async Task Load_ShouldBe_Null()
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