using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IReputationResetService
{
    /// <summary>
    ///     Resets ReputationEarnedToday of all users to min value
    /// </summary>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    Task<BaseResult> ResetEarnedTodayReputationAsync(CancellationToken cancellationToken = default);
}