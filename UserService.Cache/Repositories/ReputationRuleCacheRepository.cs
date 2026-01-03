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

public class ReputationRuleCacheRepository : IReputationRuleCacheRepository
{
    private readonly IBaseCacheRepository<ReputationRule, long> _repository;
    private readonly IGetReputationRuleService _reputationRuleInner;

    public ReputationRuleCacheRepository(ICacheProvider cacheProvider, IOptions<RedisSettings> redisSettings,
        GetReputationRuleService reputationRuleInner)
    {
        var settings = redisSettings.Value;

        _repository = new BaseCacheRepository<ReputationRule, long>(
            cacheProvider,
            x => x.Id,
            CacheKeyHelper.GetReputationRuleKey,
            x => x.Id.ToString(),
            long.Parse,
            settings.TimeToLiveInSeconds
        );
        _reputationRuleInner = reputationRuleInner;
    }

    public Task<IEnumerable<ReputationRule>> GetByIdsOrFetchAndCacheAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetByIdsOrFetchAndCacheAsync(
            ids,
            async (idsToFetch, ct) => (await _reputationRuleInner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken);
    }
}