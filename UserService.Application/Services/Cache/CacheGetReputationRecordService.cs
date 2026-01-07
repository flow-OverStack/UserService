using UserService.Application.Enums;
using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Repository.Cache;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services.Cache;

public class CacheGetReputationRecordService(
    IReputationRecordCacheRepository cacheRepository,
    GetReputationRecordService inner) : IGetReputationRecordService
{
    public Task<QueryableResult<ReputationRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetAllAsync(cancellationToken);
    }

    public async Task<CollectionResult<ReputationRecord>> GetByIdsAsync(IEnumerable<long> ids,
        CancellationToken cancellationToken = default)
    {
        var idsArray = ids.ToArray();
        var records = (await cacheRepository.GetByIdsOrFetchAndCacheAsync(idsArray, cancellationToken)).ToArray();

        if (records.Length == 0)
            return idsArray.Length switch
            {
                <= 1 => CollectionResult<ReputationRecord>.Failure(ErrorMessage.ReputationRecordNotFound,
                    (int)ErrorCodes.ReputationRecordNotFound),
                > 1 => CollectionResult<ReputationRecord>.Failure(ErrorMessage.ReputationRecordsNotFound,
                    (int)ErrorCodes.ReputationRecordsNotFound)
            };

        return CollectionResult<ReputationRecord>.Success(records);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersOwnedRecordsAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default)
    {
        var groupedRecords =
            (await cacheRepository.GetUsersOwnedRecordsOrFetchAndCacheAsync(userIds, cancellationToken))
            .ToArray();

        if (groupedRecords.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Failure(
                ErrorMessage.ReputationRecordsNotFound, (int)ErrorCodes.ReputationRecordsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Success(groupedRecords);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetUsersInitiatedRecordsAsync(IEnumerable<long> userIds, CancellationToken cancellationToken = default)
    {
        var groupedRecords =
            (await cacheRepository.GetUsersInitiatedRecordsOrFetchAndCacheAsync(userIds, cancellationToken))
            .ToArray();

        if (groupedRecords.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Failure(
                ErrorMessage.ReputationRecordsNotFound, (int)ErrorCodes.ReputationRecordsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Success(groupedRecords);
    }

    public async Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>>
        GetRecordsWithReputationRules(
            IEnumerable<long> ruleIds, CancellationToken cancellationToken = default)
    {
        var groupedRecords =
            (await cacheRepository.GetRecordsWithReputationRulesOrFetchAndCacheAsync(ruleIds, cancellationToken))
            .ToArray();

        if (groupedRecords.Length == 0)
            return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Failure(
                ErrorMessage.ReputationRecordsNotFound, (int)ErrorCodes.ReputationRecordsNotFound);

        return CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>.Success(groupedRecords);
    }
}