using Newtonsoft.Json;
using StackExchange.Redis;
using UserService.Domain.Dtos.User;

namespace UserService.Tests.Configurations;

internal static class UserActivityConfiguration
{
    public static (RedisValue[] Keys, RedisValue[] Values) GetUserActivities()
    {
        var userActivities = new UserActivityDto[]
            { new(0, DateTime.UtcNow), new(0, DateTime.UtcNow - TimeSpan.FromMinutes(1)) };
        return (
            [new RedisValue("activity:user:1"), new RedisValue("activity:user:2")],
            userActivities.Select(x => (RedisValue)JsonConvert.SerializeObject(x)).ToArray()
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetEmptyUserActivities()
    {
        return ([], []);
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetUserActivitiesWithInvalidKeys()
    {
        var userActivities = new UserActivityDto[]
            { new(0, DateTime.UtcNow), new(0, DateTime.UtcNow - TimeSpan.FromMinutes(1)) };

        return (
            [new RedisValue("WrongFormat"), new RedisValue("activity:user:wrongFormat")],
            userActivities.Select(x => (RedisValue)JsonConvert.SerializeObject(x)).ToArray()
        );
    }

    public static (RedisValue[] Keys, RedisValue[] Values) GetUserActivitiesWithInvalidValues()
    {
        var wrongUserActivitiesString = new[] { "WrongFormat", "{ Wrong Format}" };

        return (
            [new RedisValue("activity:user:1"), new RedisValue("activity:user:2")],
            wrongUserActivitiesString.Select(x => (RedisValue)JsonConvert.SerializeObject(x)).ToArray()
        );
    }
}