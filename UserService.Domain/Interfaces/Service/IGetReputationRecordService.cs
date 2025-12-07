using UserService.Domain.Entities;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IGetReputationRecordService : IGetService<ReputationRecord>
{
    /// <summary>
    ///     Gets all reputation records associated with the provided user ids
    /// </summary>
    /// <param name="userIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetUsersRecordsAsync(
        IEnumerable<long> userIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets all reputation records associated with the provided reputation rule ids
    /// </summary>
    /// <param name="ruleIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CollectionResult<KeyValuePair<long, IEnumerable<ReputationRecord>>>> GetRecordsWithReputationRules(
        IEnumerable<long> ruleIds, CancellationToken cancellationToken = default);
}