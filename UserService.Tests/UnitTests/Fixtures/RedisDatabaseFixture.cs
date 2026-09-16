using Moq;
using StackExchange.Redis;
using UserService.Cache.Helpers;
using UserService.Tests.TestData;

namespace UserService.Tests.UnitTests.Fixtures;

internal static class RedisDatabaseFixture
{
    public static IDatabase GetRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);
        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
            It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

        var activities = UserActivityCacheData.GetUserActivities();
        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
                key == CacheKeyHelper.GetUserActivitiesKey() ? activities.Keys : []);
        mockDatabase.Setup(x => x.StringGetAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey[] keys, CommandFlags _) =>
                keys.All(x => x.ToString().StartsWith(CacheKeyHelper.GetUserActivityKey(0)[..^1]))
                    ? activities.Values
                    : []);

        return mockDatabase.Object;
    }

    public static IDatabase GetEmptyRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);
        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
            It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

        var activities = UserActivityCacheData.GetEmptyUserActivities();
        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
                key == CacheKeyHelper.GetUserActivitiesKey() ? activities.Keys : []);
        mockDatabase.Setup(x => x.StringGetAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey[] keys, CommandFlags _) =>
                keys.All(x => x.ToString().StartsWith(CacheKeyHelper.GetUserActivityKey(0)[..^1]))
                    ? activities.Values
                    : []);

        return mockDatabase.Object;
    }

    public static IDatabase GetRedisDatabaseConfigurationWithInvalidKeys()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);
        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
            It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

        var activities = UserActivityCacheData.GetUserActivitiesWithInvalidKeys();
        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
                key == CacheKeyHelper.GetUserActivitiesKey() ? activities.Keys : []);
        mockDatabase.Setup(x => x.StringGetAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey[] keys, CommandFlags _) =>
                keys.All(x => x.ToString().StartsWith(CacheKeyHelper.GetUserActivityKey(0)[..^1]))
                    ? activities.Values
                    : []);

        return mockDatabase.Object;
    }

    public static IDatabase GetRedisDatabaseConfigurationWithInvalidValues()
    {
        var mockDatabase = new Mock<IDatabase>();

        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);
        mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
            It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

        var activities = UserActivityCacheData.GetUserActivitiesWithInvalidValues();
        mockDatabase.Setup(x => x.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
                key == CacheKeyHelper.GetUserActivitiesKey() ? activities.Keys : []);
        mockDatabase.Setup(x => x.StringGetAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey[] keys, CommandFlags _) =>
                keys.All(x => x.ToString().StartsWith(CacheKeyHelper.GetUserActivityKey(0)[..^1]))
                    ? activities.Values
                    : []);

        return mockDatabase.Object;
    }

    public static IDatabase GetFalseResponseRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        // Operations that return bool will return false

        return mockDatabase.Object;
    }
}
