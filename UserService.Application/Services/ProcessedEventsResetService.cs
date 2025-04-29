using Microsoft.EntityFrameworkCore;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repository;
using UserService.Domain.Interfaces.Service;
using UserService.Domain.Results;

namespace UserService.Application.Services;

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