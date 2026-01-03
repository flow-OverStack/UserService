using Microsoft.Extensions.Options;
using UserService.Application.Services;
using UserService.Cache.Helpers;
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
            x => x.Id,
            CacheKeyHelper.GetReputationRecordKey,
            x => x.Id.ToString(),
            long.Parse,
            settings.TimeToLiveInSeconds
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

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersRecordsOrFetchAndCacheAsync(
        IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(userIds,
            CacheKeyHelper.GetUserReputationRecordsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) => (await _reputationRecordInner.GetUsersRecordsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }

    public Task<IEnumerable<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetRecordsWithReputationRulesOrFetchAndCacheAsync(IEnumerable<long> ruleIds,
            CancellationToken cancellationToken = default)
    {
        return _repository.GetGroupedByOuterIdOrFetchAndCacheAsync(ruleIds,
            CacheKeyHelper.GetReputationRuleRecordsKey,
            CacheKeyHelper.GetIdFromKey,
            async (idsToFetch, ct) =>
                (await _reputationRecordInner.GetRecordsWithReputationRules(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }
}