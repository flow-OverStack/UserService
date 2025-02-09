using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entity;
using UserService.Domain.Enum;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class GraphQlService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository)
    : IGraphQlService
{
    public async Task<CollectionResult<User>> GetAllUsersAsync()
    {
        var users = await userRepository.GetAll().ToArrayAsync();
        var count = users.Length;

        if (count == 0)
            return CollectionResult<User>.Failure(ErrorMessage.UsersNotFound, (int)ErrorCodes.UsersNotFound);

        return CollectionResult<User>.Success(users, count);
    }

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

        if (count == 0)
            return CollectionResult<Role>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound);

        return CollectionResult<Role>.Success(roles, count);
    }

    public async Task<CollectionResult<User>> GetUsersWithRole(long roleId)
    {
        var role = await roleRepository.GetAll()
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id == roleId);

        if (role == null)
            return CollectionResult<User>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        var users = role.Users.ToArray();
        var count = users.Length;

        if (count == 0)
            return CollectionResult<User>.Failure(ErrorMessage.UsersNotFound, (int)ErrorCodes.UsersNotFound);

        return CollectionResult<User>.Success(users, count);
    }
}