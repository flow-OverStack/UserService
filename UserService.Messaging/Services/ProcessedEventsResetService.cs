using UserService.Domain.Results;
using UserService.Messaging.Interfaces;

namespace UserService.Messaging.Services;

public class ProcessedEventsResetService(IProcessedEventRepository eventRepository) : IProcessedEventsResetService
{
    private const int MaxProcessedEventLifetimeInDays = 7;

    public async Task<BaseResult> ResetProcessedEventsAsync(CancellationToken cancellationToken = default)
    {
        // Subtracting MaxProcessedEventLifetimeInDays
        var thresholdDate = DateTime.UtcNow.AddDays(-MaxProcessedEventLifetimeInDays);

        await eventRepository.ResetProcessedAsync(thresholdDate, cancellationToken);

        return BaseResult.Success();
    }
}