using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Service;

public interface IProcessedEventsResetService
{
    /// <summary>
    ///     Removes all processed events that are older than 1 week
    /// </summary>
    /// <returns></returns>
    /// <param name="cancellationToken"></param>
    Task<BaseResult> ResetProcessedEventsAsync(CancellationToken cancellationToken = default);
}