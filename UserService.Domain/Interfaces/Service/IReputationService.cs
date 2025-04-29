using UserService.Domain.Dtos.User;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IReputationService
{
    /// <summary>
    ///     Increases user's reputation
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<ReputationDto>> IncreaseReputationAsync(ReputationIncreaseDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Decreases user's reputation
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<ReputationDto>> DecreaseReputationAsync(ReputationDecreaseDto dto,
        CancellationToken cancellationToken = default);
}