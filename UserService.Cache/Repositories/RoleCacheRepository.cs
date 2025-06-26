using UserService.Domain.Entities;
using UserService.Domain.Helpers;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository;

namespace UserService.Cache.Repositories;

public class RoleCacheRepository : IBaseCacheRepository<Role, long>
{
    private readonly IBaseCacheRepository<Role, long> _repository;

    public RoleCacheRepository(ICacheProvider cacheProvider)
    {
        _repository = new BaseCacheRepository<Role, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetRoleKey,
            x => x.Id.ToString(),
            long.Parse
        );
    }

    public Task<IEnumerable<Role>> GetByIdsOrFetchAndCacheAsync(
        IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<Role>>> fetch,
        int timeToLiveInSeconds,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(ids, fetch, timeToLiveInSeconds, cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<Role>>>>
        GetGroupedByOuterIdOrFetchAndCacheAsync<TOuterId>(
            IEnumerable<TOuterId> outerIds,
            Func<TOuterId, string> getOuterKey,
            Func<string, TOuterId> parseOuterIdFromKey,
            Func<IEnumerable<TOuterId>, CancellationToken,
                Task<IEnumerable<KeyValuePair<TOuterId, IEnumerable<Role>>>>> fetch,
            int timeToLiveInSeconds,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(outerIds, getOuterKey, parseOuterIdFromKey, fetch,
            timeToLiveInSeconds, cancellationToken);
    }
}