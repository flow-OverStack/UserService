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

public class GetRoleService(
    IBaseRepository<User> userRepository,
    IBaseRepository<Role> roleRepository,
    IFallbackValidator<PageDto> paginationValidator)
    : IGetRoleService
{
    public async Task<PageResult<Role>> GetAllAsync(PageDto pagination,
        CancellationToken cancellationToken = default)
    {
        var validPagination = paginationValidator.GetOrFallback(pagination);
        var roles = await roleRepository.GetAll()
            .Skip(validPagination.PageNumber - 1)
            .Take(validPagination.PageSize)
            .ToListAsync(cancellationToken);

        if (!roles.Any())
            return PageResult<Role>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound);

        return PageResult<Role>.Success(roles, validPagination.PageNumber);
    }

    public async Task<CollectionResult<Role>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var roles = await roleRepository.GetAll()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
        if (!roles.Any())
            return ids.Count() switch
            {
                <= 1 => CollectionResult<Role>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound),
                > 1 => CollectionResult<Role>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound)
            };


        return CollectionResult<Role>.Success(roles);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        var groupedRoles = await userRepository.GetAll()
            .Where(x => userIds.Contains(x.Id))
            .Include(x => x.Roles)
            .Select(x => new KeyValuePair<long, IEnumerable<Role>>(x.Id, x.Roles.ToList()))
            .ToListAsync(cancellationToken);

        if (!groupedRoles.Any())
            return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Failure(ErrorMessage.RolesNotFound,
                (int)ErrorCodes.RolesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Success(groupedRoles);
    }
}