using UserService.Application.Resources;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class GetReputationRecordServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllAsync_NoFilter_ReturnsSuccess()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();

        //Act
        var result = await getReputationRecordService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIdsAsync_MixOfExistingAndNonExistentIds_ReturnsSuccess()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var recordIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getReputationRecordService.GetByIdsAsync(recordIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIdsAsync_SingleNonExistentId_ReturnsReputationRecordNotFound()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var recordIds = new List<long> { 0 };

        //Act
        var result = await getReputationRecordService.GetByIdsAsync(recordIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRecordNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIdsAsync_MultipleNonExistentIds_ReturnsReputationRecordsNotFound()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var recordIds = new List<long> { 0, 0 };

        //Act
        var result = await getReputationRecordService.GetByIdsAsync(recordIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRecordsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersOwnedRecordsAsync_MixOfExistingAndNonExistentUserIds_ReturnsSuccess()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getReputationRecordService.GetUsersOwnedRecordsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersOwnedRecordsAsync_NonExistentUserIds_ReturnsReputationRecordsNotFound()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var userIds = new List<long> { 0, 0 };

        //Act
        var result = await getReputationRecordService.GetUsersOwnedRecordsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRecordsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersInitiatedRecordsAsync_MixOfExistingAndNonExistentUserIds_ReturnsSuccess()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getReputationRecordService.GetUsersInitiatedRecordsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersInitiatedRecordsAsync_NonExistentUserIds_ReturnsReputationRecordsNotFound()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var userIds = new List<long> { 0, 0 };

        //Act
        var result = await getReputationRecordService.GetUsersInitiatedRecordsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRecordsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetRecordsWithReputationRulesAsync_MixOfExistingAndNonExistentRuleIds_ReturnsSuccess()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var ruleIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getReputationRecordService.GetRecordsWithReputationRulesAsync(ruleIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetRecordsWithReputationRulesAsync_NonExistentRuleIds_ReturnsReputationRecordsNotFound()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var ruleIds = new List<long> { 0, 0 };

        //Act
        var result = await getReputationRecordService.GetRecordsWithReputationRulesAsync(ruleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRecordsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}