using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repository.Cache;

public interface IReputationRecordCacheRepository
{
    /// <summary>
    ///     Retrieves reputation records by their IDs. If the records are not found in the cache,
    ///     they are fetched from the source, cached, and then returned.
    /// </summary>
    /// <param name="ids">The collection of IDs for which the reputation records need to be retrieved.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <return>A task representing the asynchronous operation. The task result contains the collection of reputation records.</return>
    Task<IEnumerable<ReputationRecord>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves user reputation records by their user IDs. If the records are not found in the cache,
    ///     they are fetched from the source, cached, and then returned.
    /// </summary>
    /// <param name="userIds">The collection of user IDs for which the reputation records need to be retrieved.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <return>
    ///     A task representing the asynchronous operation. The task result contains a collection of key-value pairs, where
    ///     the key is the user ID and the value is the list of corresponding reputation records.
    /// </return>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersRecordsOrFetchAndCacheAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves reputation records associated with the specified reputation rule IDs.
    ///     If the records are not found in the cache, they are fetched from the source, cached, and then returned.
    /// </summary>
    /// <param name="ruleIds">The collection of reputation rule IDs for which the reputation records need to be retrieved.</param>
    /// <param name="cancellationToken">An optional token to cancel the operation.</param>
    /// <return>
    ///     A task representing the asynchronous operation. The task result contains a collection of key-value pairs, where
    ///     the key is the reputation rule ID and the value is the corresponding reputation record.
    /// </return>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetRecordsWithReputationRulesOrFetchAndCacheAsync(
            IEnumerable<long> ruleIds, CancellationToken cancellationToken = default);
}