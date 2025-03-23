namespace UserService.ReputationConsumer.Interfaces;

public interface IProcessedEventRepository
{
    /// <summary>
    ///     Checks if event is already processed
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    Task<bool> IsEventProcessedAsync(Guid eventId);

    /// <summary>
    ///     Marks event as already processed
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    Task MarkAsProcessedAsync(Guid eventId);
}