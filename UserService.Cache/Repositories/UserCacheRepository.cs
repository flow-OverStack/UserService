using Microsoft.Extensions.Options;
using UserService.Cache.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;

namespace UserService.Cache.Repositories;

public class UserCacheRepository : IBaseCacheRepository<User, long>
{
    private readonly IBaseCacheRepository<User, long> _repository;

    public UserCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings)
    {
        var settings = redisSettings.Value;

        _repository = new BaseCacheRepository<User, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetUserKey,
            x => x.Id.ToString(),
            long.Parse,
            settings.TimeToLiveInSeconds
        );
    }

    public Task<IEnumerable<User>> GetByIdsOrFetchAndCacheAsync(
        IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<User>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<User>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<User>>>>> fetch,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(outerIds, getOuterKey, parseOuterIdFromKey, fetch,
            cancellationToken);
    }
}