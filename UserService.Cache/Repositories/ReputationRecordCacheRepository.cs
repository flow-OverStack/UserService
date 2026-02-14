using Microsoft.Extensions.Options;
using UserService.Cache.Helpers;
using UserService.Cache.Interfaces;
using UserService.Cache.Repositories.Base;
using UserService.Cache.Settings;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Provider;
using UserService.Domain.Interfaces.Repository.Cache;

namespace UserService.Cache.Repositories;

public class ReputationRecordCacheRepository : IReputationRecordCacheRepository
{
    private readonly IBaseCacheRepository<ReputationRecord, long> _repository;

    public ReputationRecordCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings)
    {
        var settings = redisSettings.Value;
        _repository = new BaseCacheRepository<ReputationRecord, long>(
            cacheProvider,
            new CacheReputationRecordMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
    }

    public Task<IEnumerable<ReputationRecord>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        Func<IEnumerable<long>, CancellationToken, Task<IEnumerable<ReputationRecord>>> fetch,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetUsersOwnedRecordsOrFetchAndCacheAsync(IEnumerable<long> userIds,
            Func<IEnumerable<long>, CancellationToken,
                Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>> fetch,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(userIds,
            CacheKeyHelper.GetUserKey,
            CacheKeyHelper.GetUserOwnedReputationRecordsKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetUsersInitiatedRecordsOrFetchAndCacheAsync(IEnumerable<long> userIds,
            Func<IEnumerable<long>, CancellationToken,
                Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>> fetch,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(userIds,
            CacheKeyHelper.GetUserKey,
            CacheKeyHelper.GetUserInitiatedReputationRecordsKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetRecordsWithReputationRulesOrFetchAndCacheAsync(IEnumerable<long> ruleIds,
            Func<IEnumerable<long>, CancellationToken,
                Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>> fetch,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(ruleIds,
            CacheKeyHelper.GetReputationRuleKey,
            CacheKeyHelper.GetReputationRuleRecordsKey,
            CacheKeyHelper.GetIdFromKey,
            fetch,
            cancellationToken);
    }

    private sealed class CacheReputationRecordMapping : ICacheEntityMapping<ReputationRecord, long>
    {
        public long GetId(ReputationRecord entity)
        {
            return entity.Id;
        }

        public string GetKey(long id)
        {
            return CacheKeyHelper.GetReputationRecordKey(id);
        }

        public string GetValue(ReputationRecord entity)
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