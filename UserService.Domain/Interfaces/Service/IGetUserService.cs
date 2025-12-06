using UserService.Domain.Entities;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IGetUserService : IGetService<User>
{
    /// <summary>
    ///     Gets all users who have the roles by their ids
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(IEnumerable<long> roleIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the current reputation of a user by its id
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, int>>> GetCurrentReputationsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the daily remaining reputation of a user by its id
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, int>>> GetRemainingReputationsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default);
}