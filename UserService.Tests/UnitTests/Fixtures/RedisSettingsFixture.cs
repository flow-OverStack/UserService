using UserService.Cache.Settings;

namespace UserService.Tests.UnitTests.Fixtures;

internal static class RedisSettingsFixture
{
    public static RedisSettings GetRedisSettingsConfiguration()
    {
        return new RedisSettings { TimeToLiveInSeconds = 300 };
    }
}