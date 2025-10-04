using StackExchange.Redis;
using UserService.Cache.Providers;
using UserService.Tests.UnitTests.Configurations;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class RedisCacheProviderTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task SetsAdd_ShouldBe_Exception()
    {
        //Arrange
        var cache = new RedisCacheProvider(
            RedisDatabaseConfiguration.GetRedisDatabaseConfiguration());
        var keysWithValues = new KeyValuePair<string, IEnumerable<string>>[]
        {
            new("key1", ["value11", "value12"]),
            new("key2", ["value21", "value22"]),
            new("key3", ["value31", "value32"])
        };

        //Act
        var action = async () => await cache.SetsAddAsync(keysWithValues, int.MaxValue);

        //Assert
        await Assert.ThrowsAsync<RedisException>(action);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task StringSet_ShouldBe_Exception()
    {
        //Arrange
        var cache = new RedisCacheProvider(
            RedisDatabaseConfiguration.GetFalseResponseRedisDatabaseConfiguration());
        var keysWithValues = new KeyValuePair<string, object>[]
        {
            new("key1", "value1"),
            new("key2", "value2"),
            new("key3", "value3")
        };

        //Act
        var action = async () => await cache.StringSetAsync(keysWithValues, int.MaxValue);

        //Assert
        await Assert.ThrowsAsync<RedisException>(action);
    }
}