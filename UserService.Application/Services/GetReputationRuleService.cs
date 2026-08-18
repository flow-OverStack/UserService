using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Extensions;
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

    public async Task<CollectionResult<ReputationRule>> GetByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken = default)
    {
        var rules = await ruleRepository.GetAll().Where(x => ids.Contains(x.Id)).ToArrayAsync(cancellationToken);

        if (rules.Length == 0) return CollectionResult<ReputationRule>.ReputationRulesNotFound(ids.Count);

        return CollectionResult<ReputationRule>.Success(rules);
    }
}