using Newtonsoft.Json;
using StackExchange.Redis;
using UserService.Domain.Interfaces.Provider;

namespace UserService.Cache.Providers;

public class RedisCacheProvider(IDatabase redisDatabase) : ICacheProvider
{
    private const string RedisErrorMessage = "An exception occurred while executing the Redis command.";

    public async Task<long> SetsAddAsync(IEnumerable<KeyValuePair<string, IEnumerable<string>>> keysWithValues,
        int timeToLiveInSeconds, bool fireAndForget = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keysWithValues);

        var commandFlags = fireAndForget
            ? CommandFlags.FireAndForget
            : CommandFlags.None;

        var keyValuePairs = keysWithValues.Where(x => x.Value.Any()).ToArray();
        var setAddTasks = keyValuePairs.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.SetAddAsync(x.Key, x.Value.Select(y => new RedisValue(y.ToString())).ToArray(),
                commandFlags);
        });
        var keyExpiresTasks = keyValuePairs.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.KeyExpireAsync(x.Key, TimeSpan.FromSeconds(timeToLiveInSeconds), commandFlags);
        });

        var setAddResult = await Task.WhenAll(setAddTasks);
        var keyExpiresResult = await Task.WhenAll(keyExpiresTasks);

        if (!fireAndForget && keyExpiresResult.Any(x => !x))
            throw new RedisException(RedisErrorMessage);

        return setAddResult.Sum();
    }

    public async Task<IEnumerable<KeyValuePair<string, IEnumerable<string>>>> SetsStringMembersAsync(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var tasks = keys.Distinct().Select(async key =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var values = await SetStringMembersAsync(key, cancellationToken);
            return new KeyValuePair<string, IEnumerable<string>>(key, values);
        });

        var results = await Task.WhenAll(tasks);

        return results;
    }

    public async Task StringSetAsync<TValue>(IEnumerable<KeyValuePair<string, TValue>> keysWithValues,
        int timeToLiveInSeconds, bool fireAndForget = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keysWithValues);

        var redisKeyWithValues = keysWithValues.DistinctBy(x => x.Key).Select(x =>
        {
            var jsonValue = JsonConvert.SerializeObject(x.Value);
            return new KeyValuePair<RedisKey, RedisValue>(x.Key, new RedisValue(jsonValue));
        });

        var commandFlags = fireAndForget
            ? CommandFlags.FireAndForget
            : CommandFlags.None;

        var tasks = redisKeyWithValues.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.StringSetAsync(x.Key, x.Value, TimeSpan.FromSeconds(timeToLiveInSeconds),
                flags: commandFlags);
        });

        var result = await Task.WhenAll(tasks);

        if (!fireAndForget && result.Any(x => !x))
            throw new RedisException(RedisErrorMessage);
    }

    public async Task<IEnumerable<T>> GetJsonParsedAsync<T>(IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var redisKeys = keys.Distinct().Select(x => (RedisKey)x).ToArray();

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.StringGetAsync(redisKeys);

        var jsonResult = new List<T>();

        foreach (var value in result)
            try
            {
                if (value.IsNull) continue;

                var jsonValue = JsonConvert.DeserializeObject<T>(value.ToString());
                if (Equals(jsonValue, default(T))) continue;

                jsonResult.Add(jsonValue!);
            }
            catch (Exception)
            {
                // If deserialization fails, we skip this value.
            }

        return jsonResult;
    }

    private async Task<IEnumerable<string>> SetStringMembersAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var values = (await redisDatabase.SetMembersAsync(key)).Select(x => x.ToString());
        return values;
    }
}