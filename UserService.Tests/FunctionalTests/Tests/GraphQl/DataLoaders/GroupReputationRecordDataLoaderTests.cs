using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupReputationRecordDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadGrouped_ShouldBe_Success()
    {
        //Arrange    
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupReputationRecordDataLoader>();
        const long userId = 2;

        //Act
        var records = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Equal(3, records.Length); // User with id 2 has 3 reputation records
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_NoRecords()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupReputationRecordDataLoader>();
        const long userId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Empty(result);
    }
}