using UserService.Application.Enums;
using UserService.Application.Extensions;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services.Cache;

public class CacheGetRoleService(IRoleCacheRepository cacheRepository, IGetRoleService inner) : IGetRoleService
{
    public QueryableResult<Role> GetAll()
    {
        return inner.GetAll();
    }

    public async Task<CollectionResult<Role>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var roles = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(ids,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (roles.Length == 0) return CollectionResult<Role>.RolesNotFound(ids.Count);

        return CollectionResult<Role>.Success(roles);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupedRoles =
            (await cacheRepository.GetUsersRolesOrFetchAndCacheAsync(userIds,
                async (idsToFetch, ct) => (await inner.GetUsersRolesAsync(idsToFetch.ToArray(), ct)).Data ?? [],
                cancellationToken)).ToArray();

        if (groupedRoles.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Failure(ErrorMessage.RolesNotFound,
                (int)ErrorCodes.RolesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Success(groupedRoles);
    }
}