using GreenDonut;
using Microsoft.Extensions.DependencyInjection;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl.DataLoaders;

public class GroupReputationRuleReputationRecordDataLoaderTests(FunctionalTestWebAppFactory factory)
    : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadGrouped_ShouldBe_Success()
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task Load_ShouldBe_NoRecords()
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