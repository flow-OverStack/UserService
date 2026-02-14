using Microsoft.Extensions.Options;
using UserService.Cache.Helpers;
using UserService.Cache.Interfaces;
using UserService.Cache.Repositories.Base;
using UserService.Cache.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository.Cache;

namespace UserService.Cache.Repositories;

public class RoleCacheRepository : IRoleCacheRepository
{
    private readonly IBaseCacheRepository<Role, long> _repository;

    public RoleCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings)
    {
        var settings = redisSettings.Value;

        _repository = new BaseCacheRepository<Role, long>(
            cacheProvider,
            new CacheRoleMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
    }

    public Task<IEnumerable<Role>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<Role>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<Role>>>> GetUsersRolesOrFetchAndCacheAsync(
        IEnumerable<long> userIds,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<KeyValuePair<long, IEnumerable<Role>>>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(userIds,
            CacheKeyHelper.GetUserKey,
            CacheKeyHelper.GetUserRolesKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
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