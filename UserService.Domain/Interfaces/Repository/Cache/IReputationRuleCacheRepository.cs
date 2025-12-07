using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repository.Cache;

public interface IReputationRuleCacheRepository
{
    /// <summary>
    ///     Retrieves reputation rules based on the provided identifiers. If the requested rules
    ///     are not available in the cache, they are fetched from the underlying data source and cached.
    /// </summary>
    /// <param name="ids">A collection of identifiers for the reputation rules to be retrieved.</param>
    /// <param name="cancellationToken">An optional token to monitor for cancellation requests.</param>
    /// <return>A task that represents the asynchronous operation. The task result contains a collection of reputation rules.</return>
    Task<IEnumerable<ReputationRule>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);
}