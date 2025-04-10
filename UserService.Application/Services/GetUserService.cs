using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entity;
using UserService.Domain.Enum;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Resources;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class GetUserService(IBaseRepository<User> userRepository, IBaseRepository<Role> roleRepository)
    : IGetUserService
{
    public async Task<CollectionResult<User>> GetAllAsync()
    {
        var users = await userRepository.GetAll().ToListAsync();

        if (!users.Any())
            return CollectionResult<User>.Failure(ErrorMessage.UsersNotFound, (int)ErrorCodes.UsersNotFound);

        return CollectionResult<User>.Success(users, users.Count);
    }

    public async Task<BaseResult<User>> GetByIdAsync(long id)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return BaseResult<User>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        return BaseResult<User>.Success(user);
    }

    public async Task<CollectionResult<User>> GetByIdsAsync(IEnumerable<long> ids)
    {
        var users = await userRepository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync();
        var totalCount = await userRepository.GetAll().CountAsync();

        if (!users.Any())
            return CollectionResult<User>.Failure(ErrorMessage.UsersNotFound, (int)ErrorCodes.UsersNotFound);

        return CollectionResult<User>.Success(users, users.Count, totalCount);
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

        return CollectionResult<User>.Success(users, count);
    }
}