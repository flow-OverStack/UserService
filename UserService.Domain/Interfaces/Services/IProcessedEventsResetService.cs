using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IProcessedEventsResetService
{
    /// <summary>
    ///     Removes all processed events that are older than 1 week
    /// </summary>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    Task<BaseResult> ResetProcessedEventsAsync(CancellationToken cancellationToken = default);
}