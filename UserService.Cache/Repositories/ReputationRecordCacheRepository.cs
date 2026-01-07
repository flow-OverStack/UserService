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

public class ReputationRecordCacheRepository : IReputationRecordCacheRepository
{
    private readonly IBaseCacheRepository<ReputationRecord, long> _repository;
    private readonly IGetReputationRecordService _reputationRecordInner;

    public ReputationRecordCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetReputationRecordService reputationRecordInner)
    {
        var settings = redisSettings.Value;

        _repository = new BaseCacheRepository<ReputationRecord, long>(
            cacheProvider,
            new CacheReputationRecordMapping(),
            settings.TimeToLiveInSeconds,
            settings.NullTimeToLiveInSeconds
        );
        _reputationRecordInner = reputationRecordInner;
    }

    public Task<IEnumerable<ReputationRecord>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            async (idsToFetch, ct) => (await _reputationRecordInner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetUsersOwnedRecordsOrFetchAndCacheAsync(
            IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(userIds,
            CacheKeyHelper.GetUserKey,
            CacheKeyHelper.GetUserOwnedReputationRecordsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) =>
                (await _reputationRecordInner.GetUsersOwnedRecordsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetUsersInitiatedRecordsOrFetchAndCacheAsync(IEnumerable<long> userIds,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(userIds,
            CacheKeyHelper.GetUserKey,
            CacheKeyHelper.GetUserInitiatedReputationRecordsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) =>
                (await _reputationRecordInner.GetUsersInitiatedRecordsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetRecordsWithReputationRulesOrFetchAndCacheAsync(IEnumerable<long> ruleIds,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(ruleIds,
            CacheKeyHelper.GetReputationRuleKey,
            CacheKeyHelper.GetReputationRuleRecordsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) =>
                (await _reputationRecordInner.GetRecordsWithReputationRules(idsToFetch, ct)).Data ?? [],
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