using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IReputationResetService
{
    /// <summary>
    ///     Resets ReputationEarnedToday of all users to min value
    /// </summary>
    /// <returns></returns>
    Task<BaseResult> ResetReputation();
}