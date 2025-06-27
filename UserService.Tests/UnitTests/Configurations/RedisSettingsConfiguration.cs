using UserService.Domain.Settings;

namespace UserService.Tests.UnitTests.Configurations;

internal static class RedisSettingsConfiguration
{
    public static RedisSettings GetRedisSettingsConfiguration()
    {
        return new RedisSettings
        {
            TimeToLiveInSeconds = 300
        };
    }
}