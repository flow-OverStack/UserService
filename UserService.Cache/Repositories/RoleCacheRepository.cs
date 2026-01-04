using Microsoft.Extensions.Options;
using UserService.Application.Services;
using UserService.Cache.Helpers;
using UserService.Cache.Interfaces;
using UserService.Cache.Repositories.Base;
using UserService.Cache.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;

namespace UserService.Cache.Repositories;

public class RoleCacheRepository : IRoleCacheRepository
{
    private readonly IBaseCacheRepository<Role, long> _repository;
    private readonly IGetRoleService _roleInner;

    public RoleCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetRoleService roleInner)
    {
        var settings = redisSettings.Value;

        _repository = new BaseCacheRepository<Role, long>(
            cacheProvider,
            new CacheRoleMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
        _roleInner = roleInner;
    }

    public Task<IEnumerable<Role>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            async (idsToFetch, ct) => (await _roleInner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesOrFetchAndCacheAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(userIds,
            CacheKeyHelper.GetUserKey,
            CacheKeyHelper.GetUserRolesKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _roleInner.GetUsersRolesAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    private sealed class CacheRoleMapping : ICacheEntityMapping<Role, long>
    {
        public long GetId(Role entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetRoleKey(id);
        }

        public string GetValue(Role entity)
        {
            return entity.Id.ToString();
        }

        public long ParseIdFromKey(string key)
        {
            return CacheKeyHelper.GetIdFromKey(key);
        }

        public long ParseIdFromValue(string value)
        {
            return long.Parse(value);
        }
    }
}