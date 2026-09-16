using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

[FunctionalTest]
public class GroupReputationRuleReputationRecordDataLoaderTests(FunctionalTestWebAppFactory factory)
    : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task LoadRequiredAsync_ExistingReputationRuleId_ReturnsRecords()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupReputationRuleReputationRecordDataLoader>();
        const long reputationRuleId = 1;

        //Act
        var records = await dataLoader.LoadRequiredAsync(reputationRuleId);

        //Assert
        Assert.Single(records); // Reputation rule with id 1 has 1 record
    }

    [Fact]
    public async Task LoadRequiredAsync_NonExistentReputationRuleId_ReturnsEmptyCollection()
    {
        //Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<GroupReputationRuleReputationRecordDataLoader>();
        const long reputationRuleId = 0;

        //Act
        var result = await dataLoader.LoadRequiredAsync(reputationRuleId);

        //Assert
        Assert.Empty(result);
    }
}