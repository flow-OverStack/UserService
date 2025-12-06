using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces.Repository.Cache;

public interface IUserCacheRepository
{
    /// <summary>
    ///     Retrieves a collection of users from the cache by their identifiers.
    ///     If any users are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the users to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{User}" /> containing the combined results from the cache and the fallback fetch, if
    ///     needed.
    /// </returns>
    Task<IEnumerable<User>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves users grouped by role identifiers from the cache.
    ///     If any role or users are missing, the data is fetched and cached accordingly.
    /// </summary>
    /// <param name="roleIds">The list of role IDs whose users should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// ///
    /// <returns>
    ///     A <see cref="IEnumerable{User}" /> containing a lookup-like list of role Ids to the list of users.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesOrFetchAndCacheAsync(
        IEnumerable<long> roleIds,
        CancellationToken cancellationToken = default);


    /// <summary>
    ///     Retrieves the current reputation scores for a collection of users by their identifiers from the cache.
    ///     If any reputations are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the users whose reputations should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{KeyValuePair{long, int}}" /> where each key-value pair represents a user ID and its
    ///     corresponding reputation score.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, int>>> GetCurrentReputationsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves the remaining reputation scores for a collection of users by their identifiers from the cache.
    ///     If any reputations are missing, they are fetched from an external source and cached.
    /// </summary>
    /// <param name="ids">The identifiers of the users whose remaining reputations should be retrieved.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="IEnumerable{KeyValuePair{long, int}}" /> containing the user identifiers and their corresponding
    ///     remaining reputations.
    /// </returns>
    Task<IEnumerable<KeyValuePair<long, int>>> GetRemainingReputationsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);
}