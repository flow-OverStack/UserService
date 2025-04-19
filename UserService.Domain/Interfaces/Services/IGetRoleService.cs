using UserService.Domain.Entity;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGetRoleService
{
    /// <summary>
    ///     Gets all of Roles
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<Role>> GetAllAsync();

    /// <summary>
    ///     Gets one Role by its id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResult<Role>> GetByIdAsync(long id);

    /// <summary>
    ///     Get all roles of the users by their ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(IEnumerable<long> userIds);
}