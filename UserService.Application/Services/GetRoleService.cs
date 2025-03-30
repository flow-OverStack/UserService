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
    public async Task<CollectionResult<Role>> GetAllAsync()
    {
        var roles = await roleRepository.GetAll().ToListAsync();

        if (!roles.Any())
            return CollectionResult<Role>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound);

        return CollectionResult<Role>.Success(roles, roles.Count);
    }

    public async Task<BaseResult<Role>> GetByIdAsync(long id)
    {
        var role = await roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

        if (role == null)
            return BaseResult<Role>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound);

        return BaseResult<Role>.Success(role);
    }

    public async Task<CollectionResult<Role>> GetByIdsAsync(IEnumerable<long> ids)
    {
        var roles = await roleRepository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync();
        var totalCount = await roleRepository.GetAll().CountAsync();

        if (!roles.Any())
            return CollectionResult<Role>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound);

        return CollectionResult<Role>.Success(roles, roles.Count, totalCount);
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