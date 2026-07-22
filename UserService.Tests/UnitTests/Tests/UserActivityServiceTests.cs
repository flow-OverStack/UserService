using UserService.Tests.UnitTests.Fixtures;
using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class UserActivityServiceTests
{
    [Fact]
    public async Task RegisterHeartbeatAsync_ExistingUserId_ReturnsSuccess()
    {
        //Arrange
        const long id = 1;
        var activityService = new UserActivityServiceSut().GetService();

        //Act
        var result = await activityService.RegisterHeartbeatAsync(id);

        //Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SyncHeartbeatsToDatabaseAsync_PendingHeartbeats_ReturnsSuccess()
    {
        //Arrange
        var activityService = new UserActivityServiceSut().GetDatabaseService();

        //Act
        var result = await activityService.SyncHeartbeatsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.SyncedHeartbeatsCount); // There are 2 new heartbeats in total
    }

    [Fact]
    public async Task SyncHeartbeatsToDatabaseAsync_EmptyHeartbeats_ReturnsNoSyncedHeartbeats()
    {
        //Arrange
        var activityService =
            new UserActivityServiceSut(RedisDatabaseFixture.GetEmptyRedisDatabaseConfiguration())
                .GetDatabaseService();

        //Act
        var result = await activityService.SyncHeartbeatsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedHeartbeatsCount);
    }

    [Fact]
    public async Task SyncHeartbeatsToDatabaseAsync_InvalidKeys_ReturnsNoSyncedHeartbeats()
    {
        //Arrange
        var activityService =
            new UserActivityServiceSut(RedisDatabaseFixture.GetRedisDatabaseConfigurationWithInvalidKeys())
                .GetDatabaseService();

        //Act
        var result = await activityService.SyncHeartbeatsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedHeartbeatsCount);
    }

    [Fact]
    public async Task SyncHeartbeatsToDatabaseAsync_InvalidValues_ReturnsNoSyncedHeartbeats()
    {
        //Arrange
        var activityService =
            new UserActivityServiceSut(RedisDatabaseFixture.GetRedisDatabaseConfigurationWithInvalidValues())
                .GetDatabaseService();

        //Act
        var result = await activityService.SyncHeartbeatsToDatabaseAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Data.SyncedHeartbeatsCount);
    }
}