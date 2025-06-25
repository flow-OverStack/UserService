namespace UserService.Domain.Interfaces.Provider;

public interface ICacheProvider
{
    /// <summary>
    ///     Adds multiple sets to the cache, where each key represents a set and its corresponding value
    ///     contains the members to be added to that set. Optionally sets a time-to-live for the keys.
    /// </summary>
    /// <param name="keysWithValues">
    ///     A collection of key-value pairs where each key identifies a set and the value is a
    ///     collection of members to add.
    /// </param>
    /// <param name="timeToLiveInSeconds">
    ///     The time-to-live (in seconds) for the keys being added. If set to zero or negative,
    ///     no expiration is applied.
    /// </param>
    /// <param name="fireAndForget">If true, sends the command in fire-and-forget mode (no result or error reported).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>The total number of members added to the sets.</returns>
    Task<long> SetsAddAsync(
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> keysWithValues,
        int timeToLiveInSeconds,
        bool fireAndForget = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the members of multiple cached sets, returning them as key-value pairs where
    ///     each key is a set identifier, and the value is the collection of members for that set.
    /// </summary>
    /// <param name="keys">A collection of cached set keys.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>A collection of key-value pairs, where the key is the set key and the value is the collection of its members.</returns>
    Task<IEnumerable<KeyValuePair<string, IEnumerable<string>>>> SetsStringMembersAsync(IEnumerable<string> keys,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously sets multiple string key-value pairs in the cache with a specified time-to-live (TTL).
    /// </summary>
    /// <typeparam name="TValue">The type of the values to be stored. Values are serialized to JSON.</typeparam>
    /// <param name="keysWithValues">A collection of key-value pairs to store in the cache.</param>
    /// <param name="timeToLiveInSeconds">The time-to-live (TTL) for each key-value pair in seconds.</param>
    /// <param name="fireAndForget">If true, sends the command in fire-and-forget mode (no result or error reported).</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StringSetAsync<TValue>(IEnumerable<KeyValuePair<string, TValue>> keysWithValues, int timeToLiveInSeconds,
        bool fireAndForget = false, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves and deserializes a collection of JSON strings from the cache into objects of type T.
    ///     Each provided key corresponds to a JSON-serialized object stored in the cache.
    /// </summary>
    /// <typeparam name="T">The type into which the JSON data will be deserialized. </typeparam>
    /// <param name="keys">A collection of keys representing the JSON-serialized objects to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests during the operation.</param>
    /// <returns>
    ///     A collection of deserialized objects of type T, corresponding to the retrieved JSON data.
    /// </returns>
    Task<IEnumerable<T>> GetJsonParsedAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default);
}