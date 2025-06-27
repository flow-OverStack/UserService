using Moq;
using StackExchange.Redis;

namespace UserService.Tests.UnitTests.Configurations;

internal static class RedisDatabaseConfiguration
{
    public static IDatabase GetRedisDatabaseConfiguration()
    {
        var mockDatabase = new Mock<IDatabase>();

        // No setups are needed for the mock IDatabase here
        // because the default return values are enough for these tests.

        return mockDatabase.Object;
    }
}