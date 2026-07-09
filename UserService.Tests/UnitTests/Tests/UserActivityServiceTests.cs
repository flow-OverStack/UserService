using UserService.Tests.UnitTests.Configurations;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class UserActivityServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterHeartbeatAsync_ExistingUserId_ReturnsSuccess()
    {
        //Arrange
        const long id = 1;
        var activityService = new UserActivityServiceFactory().GetService();

        //Act
        var result = await activityService.RegisterHeartbeatAsync(id);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncHeartbeatsToDatabaseAsync_PendingHeartbeats_ReturnsSuccess()
    {
        //Arrange
        var activityService = new UserActivityServiceFactory().GetDatabaseService();

        //Act
        var result = await activityService.SyncHeartbeatsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.SyncedHeartbeatsCount); // There are 2 new heartbeats in total
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncHeartbeatsToDatabaseAsync_EmptyHeartbeats_ReturnsNoSyncedHeartbeats()
    {
        //Arrange
        var activityService =
            new UserActivityServiceFactory(RedisDatabaseConfiguration.GetEmptyRedisDatabaseConfiguration())
                .GetDatabaseService();

        //Act
        var result = await activityService.SyncHeartbeatsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedHeartbeatsCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncHeartbeatsToDatabaseAsync_InvalidKeys_ReturnsNoSyncedHeartbeats()
    {
        //Arrange
        var activityService =
            new UserActivityServiceFactory(RedisDatabaseConfiguration.GetRedisDatabaseConfigurationWithInvalidKeys())
                .GetDatabaseService();

        //Act
        var result = await activityService.SyncHeartbeatsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedHeartbeatsCount);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task SyncHeartbeatsToDatabaseAsync_InvalidValues_ReturnsNoSyncedHeartbeats()
    {
        //Arrange
        var activityService =
            new UserActivityServiceFactory(RedisDatabaseConfiguration.GetRedisDatabaseConfigurationWithInvalidValues())
                .GetDatabaseService();

        //Act
        var result = await activityService.SyncHeartbeatsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedHeartbeatsCount);
    }
}