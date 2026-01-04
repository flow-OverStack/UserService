using Newtonsoft.Json;
using StackExchange.Redis;
using UserService.Domain.Interfaces.Provider;

namespace UserService.Cache.Providers;

public class RedisCacheProvider(IDatabase redisDatabase) : ICacheProvider
{
    private const string RedisErrorMessage = "An exception occurred while executing the Redis command.";
    private const string EntityNullKeyPattern = "{0}:null";
    private static readonly object NullValue = 1;

    public async Task<long> SetsAddAsync(IEnumerable<KeyValuePair<string, IEnumerable<string>>> keysWithValues,
        int? timeToLiveInSeconds = null, bool fireAndForget = false, CancellationToken cancellationToken = default)
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
            if (timeToLiveInSeconds == null) return Task.FromResult(true);

            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.KeyExpireAsync(x.Key, TimeSpan.FromSeconds((int)timeToLiveInSeconds), commandFlags);
        });

        var setAddResult = await Task.WhenAll(setAddTasks);
        var keyExpiresResult = await Task.WhenAll(keyExpiresTasks);

        if (!fireAndForget && keyExpiresResult.Any(x => !x))
            throw new RedisException(RedisErrorMessage);

        return setAddResult.Sum();
    }

    public Task<long> SetsAddAsync(KeyValuePair<string, IEnumerable<string>> keyWithValue,
        int? timeToLiveInSeconds = null, bool fireAndForget = false,
        CancellationToken cancellationToken = default)
    {
        return SetsAddAsync([keyWithValue], timeToLiveInSeconds, fireAndForget, cancellationToken);
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
        int? timeToLiveInSeconds = null, bool fireAndForget = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keysWithValues);

        var redisKeyWithValues = keysWithValues.DistinctBy(x => x.Key).Select(x =>
        {
            var value = x.Value as string ?? JsonConvert.SerializeObject(x.Value);

            return new KeyValuePair<RedisKey, RedisValue>(x.Key, new RedisValue(value));
        });

        var commandFlags = fireAndForget
            ? CommandFlags.FireAndForget
            : CommandFlags.None;

        var tasks = redisKeyWithValues.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.StringSetAsync(x.Key, x.Value,
                timeToLiveInSeconds != null ? TimeSpan.FromSeconds((int)timeToLiveInSeconds) : null,
                flags: commandFlags);
        });

        var result = await Task.WhenAll(tasks);

        if (!fireAndForget && result.Any(x => !x))
            throw new RedisException(RedisErrorMessage);
    }

    public Task StringSetAsync<TValue>(KeyValuePair<string, TValue> keyWithValue, int? timeToLiveInSeconds = null,
        bool fireAndForget = false,
        CancellationToken cancellationToken = default)
    {
        return StringSetAsync([keyWithValue], timeToLiveInSeconds, fireAndForget, cancellationToken);
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

    public async Task<IEnumerable<KeyValuePair<string, T>>> GetJsonParsedWithKeysAsync<T>(
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var redisKeys = keys.Distinct().Select(x => (RedisKey)x).ToArray();

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.StringGetAsync(redisKeys);

        var jsonResults = new List<KeyValuePair<string, T>>();

        for (var i = 0; i < redisKeys.Length; i++)
            try
            {
                var value = result[i];
                var key = redisKeys[i];

                if (value.IsNull) continue;

                var jsonValue = JsonConvert.DeserializeObject<T>(value!);
                if (Equals(jsonValue, default(T))) continue;

                var pair = new KeyValuePair<string, T>(key!, jsonValue!);
                jsonResults.Add(pair);
            }
            catch (Exception)
            {
                // If deserialization fails, we skip this value.
            }

        return jsonResults;
    }

    public async Task<IEnumerable<KeyValuePair<string, string>>> StringGetAsync(IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(keys);

        var redisKeys = keys.Distinct().Select(x => (RedisKey)x).ToArray();

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.StringGetAsync(redisKeys);

        var stringResults = new List<KeyValuePair<string, string>>();
        for (var i = 0; i < redisKeys.Length; i++)
        {
            var value = result[i];
            var key = redisKeys[i];

            if (value.IsNull) continue;

            stringResults.Add(new KeyValuePair<string, string>(key!, value!));
        }

        return stringResults;
    }

    public Task<long> KeysDeleteAsync(IEnumerable<string> keys, bool fireAndForget = false,
        CancellationToken cancellationToken = default)
    {
        var commandFlags = fireAndForget
            ? CommandFlags.FireAndForget
            : CommandFlags.None;

        cancellationToken.ThrowIfCancellationRequested();

        return redisDatabase.KeyDeleteAsync(keys.Select(x => (RedisKey)x).ToArray(), commandFlags);
    }

    public async Task<IEnumerable<string>> SetStringMembersAsync(string key,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var values = (await redisDatabase.SetMembersAsync(key)).Select(x => x.ToString());
        return values;
    }

    public Task MarkAsNullAsync(IEnumerable<string> keys, int? timeToLiveInSeconds = null, bool fireAndForget = false,
        CancellationToken cancellationToken = default)
    {
        var nullKeys = keys.Distinct().Select(GetNullKey);

        var kvps = nullKeys.Select(x => new KeyValuePair<string, object>(x, NullValue));

        return StringSetAsync(kvps, timeToLiveInSeconds, fireAndForget, cancellationToken);
    }

    public async Task<IReadOnlySet<string>> GetNullKeysAsync(IEnumerable<string> keys,
        CancellationToken cancellationToken = default)
    {
        var nullKeys = keys.Select(GetNullKey);

        var results = await Task.WhenAll(
            nullKeys.Select(async key =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return (Key: RemoveNullKeyPattern(key), Exists: await redisDatabase.KeyExistsAsync(key));
                }
            )
        );

        return results
            .Where(x => x.Exists)
            .Select(x => x.Key)
            .ToHashSet().AsReadOnly();
    }

    private static string GetNullKey(string key)
    {
        return string.Format(EntityNullKeyPattern, key);
    }

    private static string RemoveNullKeyPattern(string nullKey)
    {
        return nullKey.Replace(":null", string.Empty);
    }
}