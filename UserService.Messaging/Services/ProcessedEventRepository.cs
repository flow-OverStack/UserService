using Microsoft.EntityFrameworkCore;
using UserService.Domain.Interfaces.Repository;
using UserService.Messaging.Events;
using UserService.Messaging.Interfaces;

namespace UserService.Messaging.Services;

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

    public Task ResetProcessedAsync(DateTime olderThen, CancellationToken cancellationToken = default)
    {
        return eventRepository.GetAll().Where(x => x.ProcessedAt < olderThen).ExecuteDeleteAsync(cancellationToken);
    }
}