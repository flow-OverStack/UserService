using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class GroupInitiatedReputationRecordDataLoaderTests(FunctionalTestWebAppFactory factory)
    : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task LoadRequiredAsync_ExistingUserId_ReturnsRecords()
    {
        //Arrange    
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupInitiatedReputationRecordDataLoader>();
        const long userId = 1;

        //Act
        var records = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Equal(3, records.Length); // User with id 1 has 3 initiated reputation records
    }

    [Fact]
    public async Task LoadRequiredAsync_NonExistentUserId_ReturnsEmptyCollection()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupInitiatedReputationRecordDataLoader>();
        const long userId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(userId);

        //Assert
        Assert.Empty(result);
    }
}