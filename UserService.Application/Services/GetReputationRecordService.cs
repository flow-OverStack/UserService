using Microsoft.EntityFrameworkCore;
using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

public class GetReputationRecordService(IBaseRepository<ReputationRecord> recordsRepository)
    : IGetReputationRecordService
{
    public Task<QueryableResult<ReputationRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var records = recordsRepository.GetAll().Where(x => x.Enabled);

        return Task.FromResult(QueryableResult<ReputationRecord>.Success(records));
    }

    public async Task<CollectionResult<ReputationRecord>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var records = await recordsRepository.GetAll().Where(x => ids.Contains(x.Id) && x.Enabled)
            .ToArrayAsync(cancellationToken);

        if (records.Length == 0)
            return ids.Count() switch
            {
                <= 1 => CollectionResult<ReputationRecord>.Failure(ErrorMessage.ReputationRecordNotFound,
                    (int)ErrorCodes.ReputationRecordNotFound),
                > 1 => CollectionResult<ReputationRecord>.Failure(ErrorMessage.ReputationRecordsNotFound,
                    (int)ErrorCodes.ReputationRecordsNotFound)
            };

        return CollectionResult<ReputationRecord>.Success(records);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersRecordsAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var records = (await recordsRepository.GetAll()
                .Where(x => userIds.Contains(x.UserId) && x.Enabled)
                .GroupBy(x => x.UserId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<ReputationRecord>>(x.Key, x))
            .ToArray();

        if (records.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Failure(
                ErrorMessage.ReputationRecordsNotFound, (int)ErrorCodes.ReputationRecordsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Success(records);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetRecordsWithReputationRules(IEnumerable<long> ruleIds, CancellationToken cancellationToken = default)
    {
        var records = (await recordsRepository.GetAll()
                .Where(x => ruleIds.Contains(x.ReputationRuleId) && x.Enabled)
                .GroupBy(x => x.ReputationRuleId)
                .ToArrayAsync(cancellationToken))
            .Select(x => new KeyValuePair<long, IEnumerable<ReputationRecord>>(x.Key, x))
            .ToArray();

        if (records.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Failure(
                ErrorMessage.ReputationRecordsNotFound, (int)ErrorCodes.ReputationRecordsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Success(records);
    }
}