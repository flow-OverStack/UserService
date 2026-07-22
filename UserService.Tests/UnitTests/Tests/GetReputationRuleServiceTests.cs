using UserService.Application.Resources;
using UserService.Tests.UnitTests.Factories;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class GetReputationRuleServiceTests
{
    [Fact]
    public async Task GetAllReputationRules_NoFilter_ReturnsSuccess()
    {
        //Arrange
        var getReputationRuleService = new CacheGetReputationRuleServiceFactory().GetService();

        //Act
        var result = await getReputationRuleService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetReputationRulesByIds_MixOfExistingAndNonExistentIds_ReturnsSuccess()
    {
        //Arrange
        var getReputationRuleService = new CacheGetReputationRuleServiceFactory().GetService();
        var ruleIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getReputationRuleService.GetByIdsAsync(ruleIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetReputationRulesByIds_SingleNonExistentId_ReturnsReputationRuleNotFound()
    {
        //Arrange
        var getReputationRuleService = new CacheGetReputationRuleServiceFactory().GetService();
        var ruleIds = new List<long> { 0 };

        //Act
        var result = await getReputationRuleService.GetByIdsAsync(ruleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRuleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetReputationRulesByIds_MultipleNonExistentIds_ReturnsReputationRulesNotFound()
    {
        //Arrange
        var getReputationRuleService = new CacheGetReputationRuleServiceFactory().GetService();
        var ruleIds = new List<long> { 0, 0 };

        //Act
        var result = await getReputationRuleService.GetByIdsAsync(ruleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRulesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}