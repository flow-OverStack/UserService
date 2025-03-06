using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entity;
using UserService.Domain.Enum;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class GetRoleService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository)
    : IGetRoleService
{
    public async Task<CollectionResult<Role>> GetAllRolesAsync()
    {
        var roles = await roleRepository.GetAll().ToArrayAsync();
        var count = roles.Length;

        if (count == 0)
            return CollectionResult<Role>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound);

        return CollectionResult<Role>.Success(roles, count);
    }

    public async Task<CollectionResult<Role>> GetUserRoles(long userid)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userid);

        if (user == null)
            return CollectionResult<Role>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        var roles = user.Roles.ToArray();
        var count = roles.Length;

        return CollectionResult<Role>.Success(roles, count);
    }
}