using Microsoft.Extensions.Options;
using UserService.Cache.Repositories;
using UserService.Domain.Enums;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Resources;
using UserService.Domain.Results;
using UserService.Domain.Settings;
using Role = UserService.Domain.Entities.Role;

namespace UserService.Application.Services.Cache;

public class CacheGetRoleService : IGetRoleService
{
    private readonly IBaseCacheRepository<Role, long> _cacheRepository;
    private readonly IGetRoleService _inner;
    private readonly RedisSettings _redisSettings;

    public CacheGetRoleService(GetRoleService inner, ICacheProvider cacheProvider,
        IOptions<RedisSettings> redisSettings)
    {
        _cacheRepository = new BaseCacheRepository<Role, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetRoleKey,
            x => x.Id.ToString(),
            long.Parse
        );
        _inner = inner;
        _redisSettings = redisSettings.Value;
    }

    public Task<QueryableResult<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<Role>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var roles = (await _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            idsArray,
            async (idsToFetch, ct) => (await _inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

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
        var groupedRoles = (await _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserRolesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _inner.GetUsersRolesAsync(idsToFetch, ct)).Data ?? [],
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        )).ToArray();

        if (groupedRoles.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Failure(ErrorMessage.RolesNotFound,
                (int)ErrorCodes.RolesNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<Role>>>.Success(groupedRoles);
    }
}