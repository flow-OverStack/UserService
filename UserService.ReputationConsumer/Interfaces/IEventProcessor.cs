namespace UserService.ReputationConsumer.Interfaces;

public interface IEventProcessor<in TEvent> where TEvent : class
{
    Task<bool> IsEventProcessedAsync(Guid eventId);
    Task MarkAsProcessedAsync(TEvent @event);
}