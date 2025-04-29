using UserService.Domain.Entities;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IGetRoleService
{
    /// <summary>
    ///     Gets all of roles
    /// </summary>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    Task<CollectionResult<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets roles by their ids
    /// </summary>
    /// <param name="ids"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<Role>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all roles of the users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default);
}