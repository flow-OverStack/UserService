using UserService.Application.Resources;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class GetReputationRecordServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllReputationRecords_ShouldBe_Success()
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
    public async Task GetReputationRecordsByIds_ShouldBe_Success()
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
    public async Task GetReputationRecordsByIds_ShouldBe_ReputationRecordNotFound()
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
    public async Task GetReputationRecordsByIds_ShouldBe_ReputationRecordsNotFound()
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
    public async Task GetUsersReputationRecords_ShouldBe_Success()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getReputationRecordService.GetUsersRecordsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersReputationRecords_ShouldBe_ReputationRecordsNotFound()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var userIds = new List<long> { 0, 0 };

        //Act
        var result = await getReputationRecordService.GetUsersRecordsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRecordsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetReputationRecordsWithRules_ShouldBe_Success()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var ruleIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getReputationRecordService.GetRecordsWithReputationRules(ruleIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetReputationRecordsWithRules_ShouldBe_ReputationRecordsNotFound()
    {
        //Arrange
        var getReputationRecordService = new CacheGetReputationRecordServiceFactory().GetService();
        var ruleIds = new List<long> { 0, 0 };

        //Act
        var result = await getReputationRecordService.GetRecordsWithReputationRules(ruleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.ReputationRecordsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}