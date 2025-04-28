using Microsoft.EntityFrameworkCore;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repositories;
using UserService.ReputationConsumer.Interfaces;

namespace UserService.ReputationConsumer.Processors;

public class ProcessedEventRepository(IBaseRepository<ProcessedEvent> eventRepository) : IProcessedEventRepository
{
    public async Task<bool> IsEventProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await eventRepository.GetAll().AnyAsync(x => x.EventId == eventId, cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var processedEvent = new ProcessedEvent
        {
            EventId = eventId,
            ProcessedAt = DateTime.UtcNow
        };

        await eventRepository.CreateAsync(processedEvent, cancellationToken);
        await eventRepository.SaveChangesAsync(cancellationToken);
    }
}