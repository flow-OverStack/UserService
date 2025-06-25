using Microsoft.Extensions.Options;
using UserService.Cache.Repositories;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
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

    public Task<CollectionResult<Role>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        return _cacheRepository.GetByIdsOrFetchAndCacheAsync(
            ids,
            _inner.GetByIdsAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );
    }

    public Task<CollectionResult<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesAsync(IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        return _cacheRepository.GetGroupedByOuterIdOrFetchAndCacheAsync(
            userIds,
            CacheKeyHelper.GetUserRolesKey,
            CacheKeyHelper.GetIdFromKey,
            _inner.GetUsersRolesAsync,
            _redisSettings.TimeToLiveInSeconds,
            cancellationToken
        );
    }
}