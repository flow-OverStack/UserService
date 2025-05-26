using Microsoft.EntityFrameworkCore;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Interfaces.Validation;
using UserService.Domain.Resources;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class GetUserService(
    IBaseRepository<User> userRepository,
    IBaseRepository<Role> roleRepository,
    IFallbackValidator<PageDto> paginationValidator)
    : IGetUserService
{
    public async Task<PageResult<User>> GetAllAsync(PageDto pagination,
        CancellationToken cancellationToken = default)
    {
        var validPagination = paginationValidator.GetOrFallback(pagination);
        var users = await userRepository.GetAll()
            .OrderByDescending(x => x.CreatedAt)
            .Skip((validPagination.PageNumber - 1) * validPagination.PageSize)
            .Take(validPagination.PageSize)
            .ToListAsync(cancellationToken);
        var totalCount = await userRepository.GetAll().CountAsync(cancellationToken);

        return PageResult<User>.Success(users, validPagination.PageNumber, totalCount);
    }

    public async Task<BaseResult<User>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAll()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (user == null)
            return BaseResult<User>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound);

        return BaseResult<User>.Success(user);
    }

    public async Task<CollectionResult<User>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAll().Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        if (!users.Any())
            return ids.Count() switch
            {
                <= 1 => CollectionResult<User>.Failure(ErrorMessage.UserNotFound, (int)ErrorCodes.UserNotFound),
                > 1 => CollectionResult<User>.Failure(ErrorMessage.UsersNotFound, (int)ErrorCodes.UsersNotFound)
            };

        return CollectionResult<User>.Success(users);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<User>>>> GetUsersWithRolesAsync(
        IEnumerable<long> roleIds, CancellationToken cancellationToken = default)
    {
        var groupedUsers = await roleRepository.GetAll()
            .Where(x => roleIds.Contains(x.Id))
            .Include(x => x.Users)
            .Select(x => new KeyValuePair<long, IEnumerable<User>>(x.Id, x.Users.ToList()))
            .ToListAsync(cancellationToken);

        if (!groupedUsers.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Failure(ErrorMessage.UsersNotFound,
                (int)ErrorCodes.UsersNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<User>>>.Success(groupedUsers);
    }
}