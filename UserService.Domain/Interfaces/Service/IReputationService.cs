using UserService.Domain.Dtos.User;
using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IReputationService
{
    /// <summary>
    ///     Applies a reputation event to a user/users based on the specified event data.
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="cancellationToken"></param>
    Task<BaseResult> ApplyReputationEventAsync(ReputationEventDto dto,
        CancellationToken cancellationToken = default);
}