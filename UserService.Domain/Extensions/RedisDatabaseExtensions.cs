using Newtonsoft.Json;
using StackExchange.Redis;

namespace UserService.Domain.Extensions;

public static class RedisDatabaseExtensions
{
    private const string RedisErrorMessage = "An exception occurred while executing the Redis command.";

    /// <summary>
    ///     Adds multiple sets to Redis, where each key represents a set and its corresponding value
    ///     contains the members to be added to that set. Optionally sets a time-to-live for the keys.
    /// </summary>
    /// <param name="redisDatabase">The Redis database where the operation is performed.</param>
    /// <param name="keysWithValues">
    ///     A collection of key-value pairs where each key identifies a set and the value is a
    ///     collection of members to add.
    /// </param>
    /// <param name="timeToLiveInSeconds">
    ///     The time-to-live (in seconds) for the keys being added. If set to zero or negative,
    ///     no expiration is applied.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>The total number of members added to the sets.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="redisDatabase" /> or
    ///     <paramref name="keysWithValues" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="RedisException">Thrown when setting expiration fails for any of the keys.</exception>
    public static async Task<long> SetsAddAsync(this IDatabase redisDatabase,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> keysWithValues,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentNullException.ThrowIfNull(keysWithValues);

        var keyValuePairs = keysWithValues.Where(x => x.Value.Any()).ToList();
        var setAddTasks = keyValuePairs.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.SetAddAsync(x.Key, x.Value.Select(y => new RedisValue(y.ToString())).ToArray());
        });
        var keyExpiresTasks = keyValuePairs.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.KeyExpireAsync(x.Key,
                TimeSpan.FromSeconds(timeToLiveInSeconds));
        });

        var setAddResult = await Task.WhenAll(setAddTasks);
        var keyExpiresResult = await Task.WhenAll(keyExpiresTasks);

        if (keyExpiresResult.Any(x => !x))
            throw new RedisException(RedisErrorMessage);

        return setAddResult.Sum();
    }

    public static Task<long> SetsAddAsync(this IDatabase redisDatabase,
        KeyValuePair<string, IEnumerable<string>> keyWithValues,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default)
    {
        return redisDatabase.SetsAddAsync([keyWithValues], timeToLiveInSeconds, cancellationToken);
    }


    /// <summary>
    ///     Retrieves the members of a set in Redis as a collection of strings.
    ///     The set is identified by the provided Redis key.
    /// </summary>
    /// <param name="redisDatabase">The Redis database to perform the operation on.</param>
    /// <param name="key">The Redis key that represents the set.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>A collection of strings representing the members of the set.</returns>
    public static async Task<IEnumerable<string>> SetStringMembersAsync(this IDatabase redisDatabase, RedisKey key,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        cancellationToken.ThrowIfCancellationRequested();

        var values = (await redisDatabase.SetMembersAsync(key)).Select(x => x.ToString());
        return values;
    }

    /// <summary>
    ///     Retrieves the members of multiple Redis sets, returning them as key-value pairs where
    ///     each key is a set identifier, and the value is the collection of members for that set.
    /// </summary>
    /// <param name="redisDatabase">The Redis database to perform the operation on.</param>
    /// <param name="keys">A collection of Redis set keys.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>A collection of key-value pairs, where the key is the set key and the value is the collection of its members.</returns>
    public static async Task<IEnumerable<KeyValuePair<string, IEnumerable<string>>>> SetsStringMembersAsync(
        this IDatabase redisDatabase, IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentNullException.ThrowIfNull(keys);

        var tasks = keys.Distinct().Select(async key =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var values = await redisDatabase.SetStringMembersAsync(key, cancellationToken);
            return new KeyValuePair<string, IEnumerable<string>>(key, values);
        });

        var results = await Task.WhenAll(tasks);

        return results;
    }

    /// <summary>
    ///     Asynchronously sets multiple string key-value pairs in Redis with a specified time-to-live (TTL).
    /// </summary>
    /// <typeparam name="TValue">The type of the values to be stored. Values are serialized to JSON.</typeparam>
    /// <param name="redisDatabase">The Redis database instance to operate on.</param>
    /// <param name="keysWithValues">A collection of key-value pairs to store in Redis.</param>
    /// <param name="timeToLiveInSeconds">The time-to-live (TTL) for each key-value pair in seconds.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="redisDatabase" /> or
    ///     <paramref name="keysWithValues" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="RedisException">Thrown if any of the key-value pairs fail to be set in Redis.</exception>
    public static async Task StringSetAsync<TValue>(this IDatabase redisDatabase,
        IEnumerable<KeyValuePair<string, TValue>> keysWithValues,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentNullException.ThrowIfNull(keysWithValues);

        var redisKeyWithValues = keysWithValues.DistinctBy(x => x.Key).Select(x =>
        {
            var jsonValue = JsonConvert.SerializeObject(x.Value);
            return new KeyValuePair<RedisKey, RedisValue>(x.Key, new RedisValue(jsonValue));
        });

        var tasks = redisKeyWithValues.Select(x =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return redisDatabase.StringSetAsync(x.Key, x.Value, TimeSpan.FromSeconds(timeToLiveInSeconds));
        });

        var result = await Task.WhenAll(tasks);

        if (result.Any(x => !x))
            throw new RedisException(RedisErrorMessage);
    }

    /// <summary>
    ///     Retrieves and deserializes a collection of JSON strings from Redis into objects of type T.
    ///     Each provided key corresponds to a JSON-serialized object stored in Redis.
    /// </summary>
    /// <typeparam name="T">
    ///     The type into which the JSON data will be deserialized.
    ///     Must be a class with a parameterless constructor.
    /// </typeparam>
    /// <param name="redisDatabase">The Redis database to query for the JSON data.</param>
    /// <param name="keys">A collection of Redis keys representing the JSON-serialized objects to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>
    ///     A collection of deserialized objects of type T, corresponding to the retrieved JSON data.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the redisDatabase or keys argument is null.</exception>
    public static async Task<IEnumerable<T>> GetJsonParsedAsync<T>(this IDatabase redisDatabase,
        IEnumerable<string> keys,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentNullException.ThrowIfNull(keys);

        var redisKeys = keys.Distinct().Select(x => (RedisKey)x).ToArray();

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.StringGetAsync(redisKeys);

        var jsonResult = new List<T>();

        foreach (var value in result)
        {
            if (value.IsNull) continue;

            var jsonValue = JsonConvert.DeserializeObject<T>(value.ToString());

            if (jsonValue == null) continue;

            jsonResult.Add(jsonValue);
        }

        return jsonResult;
    }

    /// <summary>
    ///     Retrieves and deserializes JSON string from Redis into object of type T.
    ///     Key corresponds to a JSON-serialized object stored in Redis.
    /// </summary>
    /// <typeparam name="T">
    ///     The type into which the JSON data will be deserialized.
    ///     Must be a class with a parameterless constructor.
    /// </typeparam>
    /// <param name="redisDatabase">The Redis database to query for the JSON data.</param>
    /// <param name="key">Redis key representing the JSON-serialized object to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>
    ///     Deserialized object of type T, corresponding to the retrieved JSON data.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the redisDatabase or keys argument is null.</exception>
    public static async Task<T?> GetJsonParsedOrDefaultAsync<T>(this IDatabase redisDatabase,
        string key, CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(redisDatabase);
        ArgumentNullException.ThrowIfNull(key);

        var redisKey = (RedisKey)key;

        cancellationToken.ThrowIfCancellationRequested();

        var result = await redisDatabase.StringGetAsync(redisKey);

        if (result.IsNull) return null;

        var jsonResult = JsonConvert.DeserializeObject<T>(result.ToString());

        return jsonResult;
    }
}