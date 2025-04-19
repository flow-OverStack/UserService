using UserService.Domain.Entity;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGetRoleService
{
    /// <summary>
    ///     Gets all of roles
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<Role>> GetAllAsync();

    /// <summary>
    ///     Gets roles by their ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<CollectionResult<Role>> GetByIdsAsync(IEnumerable<long> ids);

    /// <summary>
    ///     Gets all roles of the users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(IEnumerable<long> userIds);
}