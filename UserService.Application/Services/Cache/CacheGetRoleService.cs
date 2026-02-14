using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services.Cache;

public class CacheGetRoleService(IRoleCacheRepository cacheRepository, IGetRoleService inner) : IGetRoleService
{
    public Task<QueryableResult<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<Role>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var roles = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (roles.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<Role>.Failure(ErrorMessage.RoleNotFound, (int)ErrorCodes.RoleNotFound),
                > 1 => CollectionResult<Role>.Failure(ErrorMessage.RolesNotFound, (int)ErrorCodes.RolesNotFound)
            };

        return CollectionResult<Role>.Success(roles);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupedRoles =
            (await cacheRepository.GetUsersRolesOrFetchAndCacheAsync(userIds,
                async (idsToFetch, ct) => (await inner.GetUsersRolesAsync(idsToFetch, ct)).Data ?? [],
                cancellationToken)).ToArray();

        if (groupedRoles.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Failure(ErrorMessage.RolesNotFound,
                (int)ErrorCodes.RolesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Success(groupedRoles);
    }
}