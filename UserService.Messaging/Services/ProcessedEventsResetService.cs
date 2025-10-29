using Microsoft.EntityFrameworkCore;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Results;
using UserService.Messaging.Events;
using UserService.Messaging.Interfaces;

namespace UserService.Messaging.Services;

public class ProcessedEventsResetService(IBaseRepository<ProcessedEvent> eventRepository) : IProcessedEventsResetService
{
    private const int MaxProcessedEventLifetimeInDays = 7;

    public async Task<BaseResult> ResetProcessedEventsAsync(CancellationToken cancellationToken = default)
    {
        // Subtracting MaxProcessedEventLifetimeInDays
        var thresholdDate = DateTime.UtcNow.AddDays(-MaxProcessedEventLifetimeInDays);

        await eventRepository.GetAll().Where(x => x.ProcessedAt <= thresholdDate).ExecuteDeleteAsync(cancellationToken);

        await eventRepository.SaveChangesAsync(cancellationToken);

        return BaseResult.Success();
    }
}