using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Services;

public interface IProcessedEventsResetService
{
    /// <summary>
    ///     Removes all processed events that are older than 1 week
    /// </summary>
    /// <returns></returns>
    Task<BaseResult> ResetProcessedEventsAsync();
}