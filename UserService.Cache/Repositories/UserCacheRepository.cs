using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;

namespace UserService.Cache.Repositories;

public class UserCacheRepository : IBaseCacheRepository<User, long>
{
    private readonly IBaseCacheRepository<User, long> _repository;

    public UserCacheRepository(ICacheProvider cacheProvider)
    {
        _repository = new BaseCacheRepository<User, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetUserKey,
            x => x.Id.ToString(),
            long.Parse
        );
    }

    public Task<IEnumerable<User>> GetByIdsOrFetchAndCacheAsync(
        IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<User>>> fetch,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, timeToLiveInSeconds, cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<User>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<User>>>>> fetch,
            int timeToLiveInSeconds,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(outerIds, getOuterKey, parseOuterIdFromKey, fetch,
            timeToLiveInSeconds, cancellationToken);
    }
}