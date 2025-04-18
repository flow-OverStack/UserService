using UserService.Domain.Entity;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IGetUserService : IGetService<User>
{
    /// <summary>
    ///     Get all users who have the roles by their ids
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRoles(IEnumerable<long> roleIds);
}