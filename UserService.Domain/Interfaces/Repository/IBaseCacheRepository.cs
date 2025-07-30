namespace UserService.Domain.Interfaces.Repository;

public interface IBaseCacheRepository<TEntity, TEntityId>
{
    /// <summary>
    ///     Retrieves a collection of entities from the cache by their identifiers. 
    ///     If any entities are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the entities to retrieve.</param>
    /// <param name="fetch">A function that fetches missing entities from an external source.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{TEntity}"/> containing the combined results from the cache and the fallback fetch, if needed.
    /// </returns>
    Task<IEnumerable<TEntity>> GetByIdsOrFetchAndCacheAsync(IEnumerable<TEntityId> ids,
        Func<IEnumerable<TEntityId>, CancellationToken, Task<IEnumerable<TEntity>>> fetch,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves entities grouped by an outer identifier (e.g. tagId -> questions) from the cache.
    ///     If any group or group members are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <typeparam name="TOuterId">The type of the outer grouping identifier.</typeparam>
    /// <param name="outerIds">The list of outer IDs whose group data should be retrieved.</param>
    /// <param name="getOuterKey">A function that maps an outer ID to a cache set key.</param>
    /// <param name="parseOuterIdFromKey">A function that parses the outer ID from the cache key.</param>
    /// <param name="fetch">A function that fetches grouped data from an external source.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{TEntity}"/> containing a lookup-like list of outer ID to the list of entities.
    /// </returns>
    Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<TEntity>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<TEntity>>>>> fetch,
            CancellationToken cancellationToken = default
        );
}