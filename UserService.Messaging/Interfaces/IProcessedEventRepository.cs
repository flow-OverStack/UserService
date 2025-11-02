namespace UserService.Messaging.Interfaces;

public interface IProcessedEventRepository
{
    /// <summary>
    ///     Checks if event is already processed
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> IsEventProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Marks event as already processed
    /// </summary>
    /// <param name="eventId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Resets the processed events were processed before the specified date.
    /// </summary>
    /// <param name="olderThen"></param>
    /// <param name="cancellationToken"></param>
    Task ResetProcessedAsync(DateTime olderThen, CancellationToken cancellationToken = default);
}