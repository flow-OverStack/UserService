using UserService.Application.Enums;
using UserService.Application.Extensions;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services.Cache;

public class CacheGetReputationRuleService(
    IReputationRuleCacheRepository cacheRepository,
    IGetReputationRuleService inner) : IGetReputationRuleService
{
    public Task<QueryableResult<ReputationRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<ReputationRule>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var rules = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch.ToArray(), ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (rules.Length == 0) return CollectionResult<ReputationRule>.ReputationRulesNotFound(idsArray.Length);

        return CollectionResult<ReputationRule>.Success(rules);
    }
}