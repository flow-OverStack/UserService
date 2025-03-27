using UserService.Domain.Entity;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGetRoleService : IGetService<Role>
{
    /// <summary>
    ///     Get all roles of the user by his id
    /// </summary>
    /// <param name="userid"></param>
    /// <returns></returns>
    Task<CollectionResult<Role>> GetUserRoles(long userid);
}