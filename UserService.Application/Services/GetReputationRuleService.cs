using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class GetReputationRuleService(IBaseRepository<ReputationRule> ruleRepository) : IGetReputationRuleService
{
    public Task<QueryableResult<ReputationRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var rules = ruleRepository.GetAll();

        return Task.FromResult(QueryableResult<ReputationRule>.Success(rules));
    }

    public async Task<CollectionResult<ReputationRule>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var rules = await ruleRepository.GetAll().Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken);

        if (rules.Length == 0)
            return ids.Count() switch
            {
                <= 1 => CollectionResult<ReputationRule>.Failure(ErrorMessage.ReputationRuleNotFound,
                    (int)ErrorCodes.ReputationRuleNotFound),
                > 1 => CollectionResult<ReputationRule>.Failure(ErrorMessage.ReputationRulesNotFound,
                    (int)ErrorCodes.ReputationRulesNotFound)
            };

        return CollectionResult<ReputationRule>.Success(rules);
    }
}