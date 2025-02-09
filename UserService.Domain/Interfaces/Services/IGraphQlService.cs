using UserService.Domain.Entity;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGraphQlService
{
    /// <summary>
    ///     Get all users
    /// </summary>
    /// <returns></returns>
    Task<CollectionResult<User>> GetAllUsersAsync();

    Task<CollectionResult<Role>> GetAllRolesAsync();

    /// <summary>
    ///     Get all roles of the user by his id
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    Task<CollectionResult<Role>> GetUserRoles(long userid);

    /// <summary>
    ///     Get all users who have the role by its id
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<CollectionResult<User>> GetUsersWithRole(long roleId);
}