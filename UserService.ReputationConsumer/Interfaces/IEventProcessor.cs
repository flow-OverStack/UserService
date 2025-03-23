namespace UserService.ReputationConsumer.Interfaces;

public interface IEventProcessor<in TEvent> where TEvent : class
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
    /// <param name="event"></param>
    /// <returns></returns>
    Task MarkAsProcessedAsync(TEvent @event);
}