using StackExchange.Redis;
using UserService.Cache.Helpers;
using UserService.Domain.Interfaces.Provider;
using UserService.Tests.Configurations;

namespace UserService.Tests.FunctionalTests.Helpers;

internal static class UserActivityHelper
{
    public static Task InsertActivities(this ICacheProvider redisDatabase)
    {
        var activities = UserActivityConfiguration.GetUserActivities();

        return redisDatabase.InsertActivitiesFromKeysAndValues(activities);
    }

    public static Task InsertInvalidActivities(this ICacheProvider redisDatabase)
    {
        var activities = UserActivityConfiguration.GetUserActivitiesWithInvalidValues();

        return redisDatabase.InsertActivitiesFromKeysAndValues(activities);
    }

    private static async Task InsertActivitiesFromKeysAndValues(this ICacheProvider redisDatabase,
        (RedisValue[] Keys, RedisValue[] Values) activities)
    {
        var keyValues = activities.Keys.Select((x, i) => new KeyValuePair<string, string>(x!, activities.Values[i]!));

        await redisDatabase.SetsAddAsync(
            new KeyValuePair<string, IEnumerable<string>>(CacheKeyHelper.GetUserActivitiesKey(),
                activities.Keys.Select(x => x.ToString())));
        await redisDatabase.StringSetAsync(keyValues);
    }
}