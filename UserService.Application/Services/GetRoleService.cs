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

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRoles(
        IEnumerable<long> userIds)
    {
        var groupedRoles = await userRepository.GetAll()
            .Where(x => userIds.Contains(x.Id))
            .Include(x => x.Roles)
            .Select(x => new KeyValuePair<long, IEnumerable<Role>>(x.Id, x.Roles.ToArray()))
            .ToListAsync();

        if (!groupedRoles.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Failure(ErrorMessage.RolesNotFound,
                (int)ErrorCodes.RolesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Success(groupedRoles, groupedRoles.Count);
    }
}