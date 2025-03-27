using UserService.Domain.Entity;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGetUserService : IGetService<User>
{
    /// <summary>
    ///     Get all users who have the role by its id
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<CollectionResult<User>> GetUsersWithRole(long roleId);
}