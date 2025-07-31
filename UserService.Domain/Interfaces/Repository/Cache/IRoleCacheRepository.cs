using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repository.Cache;

public interface IRoleCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of roles from the cache by their identifiers.
    ///     If any roles are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the roles to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{Role}" /> containing the combined results from the cache and the fallback fetch, if
    ///     needed.
    /// </returns>
    Task<IEnumerable<Role>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves roles grouped by user identifiers from the cache.
    ///     If any user or roles are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="userIds">The list of user IDs whose users should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{User}" /> containing a lookup-like list of user Ids to the list of roles.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesOrFetchAndCacheAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default);
}