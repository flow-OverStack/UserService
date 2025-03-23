using Microsoft.EntityFrameworkCore;
using UserService.Domain.Events;
using UserService.Domain.Interfaces.Repositories;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Result;

namespace UserService.Application.Services;

public class ProcessedEventsResetService(IBaseRepository<ProcessedEvent> eventRepository) : IProcessedEventsResetService
{
    private const int MaxProcessedEventLifetimeInDays = 7;

    public async Task<BaseResult> ResetProcessedEventsAsync()
    {
        // Subtracting MaxProcessedEventLifetimeInDays
        var thresholdDate = DateTime.UtcNow.AddDays(-MaxProcessedEventLifetimeInDays);

        await eventRepository.GetAll().Where(x => x.ProcessedAt <= thresholdDate).ExecuteDeleteAsync();

        await eventRepository.SaveChangesAsync();

        return BaseResult.Success();
    }
}