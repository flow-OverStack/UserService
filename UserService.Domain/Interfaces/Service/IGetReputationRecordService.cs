using UserService.Domain.Entities;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IGetReputationRecordService : IGetService<ReputationRecord>
{
    /// <summary>
    ///     Gets all owned reputation records associated with the provided user ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersOwnedRecordsAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all initiated reputation records associated with the provided user ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersInitiatedRecordsAsync(
        IReadOnlyCollection<long> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all reputation records associated with the provided reputation rule ids
    /// </summary>
    /// <param name="ruleIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetRecordsWithReputationRulesAsync(
        IReadOnlyCollection<long> ruleIds, CancellationToken cancellationToken = default);
}