using Microsoft.Extensions.Options;
using UserService.Cache.Helpers;
using UserService.Cache.Interfaces;
using UserService.Cache.Repositories.Base;
using UserService.Cache.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository.Cache;

namespace UserService.Cache.Repositories;

public class ReputationRuleCacheRepository : IReputationRuleCacheRepository
{
    private readonly IBaseCacheRepository<ReputationRule, long> _repository;

    public ReputationRuleCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings)
    {
        var settings = redisSettings.Value;

        _repository = new BaseCacheRepository<ReputationRule, long>(
            cacheProvider,
            new CacheReputationRuleMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
    }

    public Task<IEnumerable<ReputationRule>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<ReputationRule>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            fetch,
            cancellationToken);
    }

    private sealed class CacheReputationRuleMapping : ICacheEntityMapping<ReputationRule, long>
    {
        public long GetId(ReputationRule entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetReputationRuleKey(id);
        }

        public string GetValue(ReputationRule entity)
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