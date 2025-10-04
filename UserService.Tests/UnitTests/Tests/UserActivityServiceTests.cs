using UserService.Tests.UnitTests.Configurations;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class UserActivityServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterHeartbeat_ShouldBe_Success()
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
    public async Task SyncHeartbeatsToDatabase_ShouldBe_Success()
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
    public async Task SyncHeartbeatsToDatabase_ShouldBe_NoSyncedHeartbeats()
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
    public async Task SyncHeartbeatsToDatabase_ShouldBe_NoSyncedHeartbeats_With_KeysInvalid()
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
    public async Task SyncHeartbeatsToDatabase_ShouldBe_NoSyncedHeartbeats_With_ValuesInvalid()
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