using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entity.Events;
using UserService.Domain.Interfaces.Repositories;
using UserService.ReputationConsumer.Events;
using UserService.ReputationConsumer.Interfaces;

namespace UserService.ReputationConsumer.Processors;

public class BaseEventProcessor(IBaseRepository<ProcessedEvent> eventRepository) : IEventProcessor<BaseEvent>
{
    public async Task<bool> IsEventProcessedAsync(Guid eventId)
    {
        return await eventRepository.GetAll().AnyAsync(x => x.EventId == eventId);
    }

    public async Task MarkAsProcessedAsync(BaseEvent @event)
    {
        var processedEvent = new ProcessedEvent
        {
            EventId = @event.EventId,
            ProcessedAt = DateTime.UtcNow
        };

        await eventRepository.CreateAsync(processedEvent);
        await eventRepository.SaveChangesAsync();
    }
}