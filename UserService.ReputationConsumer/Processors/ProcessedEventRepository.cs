using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entity.Events;
using UserService.Domain.Interfaces.Repositories;
using UserService.ReputationConsumer.Interfaces;

namespace UserService.ReputationConsumer.Processors;

public class ProcessedEventRepository(IBaseRepository<ProcessedEvent> eventRepository) : IProcessedEventRepository
{
    public async Task<bool> IsEventProcessedAsync(Guid eventId)
    {
        return await eventRepository.GetAll().AnyAsync(x => x.EventId == eventId);
    }

    public async Task MarkAsProcessedAsync(Guid eventId)
    {
        var processedEvent = new ProcessedEvent
        {
            EventId = eventId,
            ProcessedAt = DateTime.UtcNow
        };

        await eventRepository.CreateAsync(processedEvent);
        await eventRepository.SaveChangesAsync();
    }
}