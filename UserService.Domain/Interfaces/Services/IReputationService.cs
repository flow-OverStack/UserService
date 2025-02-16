using UserService.Domain.Dto.User;
using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IReputationService
{
    /// <summary>
    ///     Increases user's reputation
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<ReputationDto>> IncreaseReputationAsync(ReputationIncreaseDto dto);

    /// <summary>
    ///     Decreases user's reputation
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<BaseResult<ReputationDto>> DecreaseReputationAsync(ReputationDecreaseDto dto);
}