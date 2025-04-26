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
            return ids.Count() switch
            {
                <= 1 => CollectionResult<User>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound),
                > 1 => CollectionResult<User>.Failure(ErrorMessage.UsersNotFound, (int)ErrorCodes.UsersNotFound)
            };

        return CollectionResult<User>.Success(users, users.Count, totalCount);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IEnumerable<long> roleIds)
    {
        var groupedUsers = await roleRepository.GetAll()
            .Where(x => roleIds.Contains(x.Id))
            .Include(x => x.Users)
            .Select(x => new KeyValuePair<long, IEnumerable<User>>(x.Id, x.Users.ToArray()))
            .ToListAsync();

        if (!groupedUsers.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Failure(ErrorMessage.UsersNotFound,
                (int)ErrorCodes.UsersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(groupedUsers, groupedUsers.Count);
    }
}