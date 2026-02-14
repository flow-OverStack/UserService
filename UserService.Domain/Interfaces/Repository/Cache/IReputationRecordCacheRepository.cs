using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repository.Cache;

public interface IReputationRecordCacheRepository
{
    /// <summary>
    ///     Retrieves reputation records by their IDs. If the records are not found in the cache,
    ///     they are fetched using <paramref name="fetch"/>, cached, and then returned.
    /// </summary>
    /// <param name="ids">The collection of IDs for which the reputation records need to be retrieved.</param>
    /// <param name="fetch"> A fallback delegate that fetches missing questions by their identifiers.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the collection of reputation records.</return>
    Task<IEnumerable<ReputationRecord>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<ReputationRecord>>> fetch,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves user-owned reputation records by their user IDs. If the records are not found in the cache,
    ///     they are fetched using <paramref name="fetch"/>, cached, and then returned.
    /// </summary>
    /// <param name="userIds">The collection of user IDs for which the reputation records need to be retrieved.</param>
    /// <param name="fetch"> A fallback delegate that fetches missing questions by their identifiers.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <return>
    ///     A task representing the asynchronous operation. The task result contains a collection of key-value pairs, where
    ///     the key is the user ID and the value is the list of corresponding reputation records.
    /// </return>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersOwnedRecordsOrFetchAndCacheAsync(
        IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>>
            fetch, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves user initiated reputation records by their user IDs. If the records are not found in the cache,
    ///     they are fetched using <paramref name="fetch"/>, cached, and then returned.
    /// </summary>
    /// <param name="userIds">The collection of user IDs for which the reputation records need to be retrieved.</param>
    /// <param name="fetch"> A fallback delegate that fetches missing questions by their identifiers.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <return>
    ///     A task representing the asynchronous operation. The task result contains a collection of key-value pairs, where
    ///     the key is the user ID and the value is the list of corresponding reputation records.
    /// </return>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersInitiatedRecordsOrFetchAndCacheAsync(
        IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>>
            fetch, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves reputation records associated with the specified reputation rule IDs.
    ///     If the records are not found in the cache, they are fetched using <paramref name="fetch"/>, cached, and then returned.
    /// </summary>
    /// <param name="ruleIds">The collection of reputation rule IDs for which the reputation records need to be retrieved.</param>
    /// <param name="fetch"> A fallback delegate that fetches missing questions by their identifiers.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <return>
    ///     A task representing the asynchronous operation. The task result contains a collection of key-value pairs, where
    ///     the key is the reputation rule ID and the value is the corresponding reputation record.
    /// </return>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetRecordsWithReputationRulesOrFetchAndCacheAsync(IEnumerable<long> ruleIds,
            Func<IEnumerable<long>, CancellationToken,
                Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>> fetch,
            CancellationToken cancellationToken = default);
}