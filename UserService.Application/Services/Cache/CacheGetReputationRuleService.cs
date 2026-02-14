using UserService.Application.Enums;
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

    public async Task<CollectionResult<ReputationRule>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var rules = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(idsArray,
            async (idsToFetch, ct) => (await inner.GetByIdsAsync(idsToFetch, ct)).Data ?? [],
            cancellationToken)).ToArray();

        if (rules.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<ReputationRule>.Failure(ErrorMessage.ReputationRuleNotFound,
                    (int)ErrorCodes.ReputationRuleNotFound),
                > 1 => CollectionResult<ReputationRule>.Failure(ErrorMessage.ReputationRulesNotFound,
                    (int)ErrorCodes.ReputationRulesNotFound)
            };

        return CollectionResult<ReputationRule>.Success(rules);
    }
}