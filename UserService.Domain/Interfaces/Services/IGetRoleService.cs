using UserService.Domain.Entity;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGetRoleService
{
    /// <summary>
    ///     Get all roles
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<Role>> GetAllRolesAsync();

    /// <summary>
    ///     Get all roles of the user by his id
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    Task<CollectionResult<Role>> GetUserRoles(long userid);
}