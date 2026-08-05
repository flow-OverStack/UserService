using UserService.Messaging.Events;

namespace UserService.Tests.TestData;

internal static class ProcessedEventMother
{
    public static IQueryable<ProcessedEvent> GetProcessedEvents()
    {
        return new[]
        {
            new ProcessedEvent
            {
                EventId = Guid.NewGuid(),
                ProcessedAt = DateTime.UtcNow
            },
            new ProcessedEvent
            {
                EventId = Guid.NewGuid(),
                ProcessedAt = DateTime.UtcNow.AddDays(-7)
            },
            new ProcessedEvent
            {
                EventId = Guid.NewGuid(),
                ProcessedAt = DateTime.UtcNow.AddDays(-7)
            }
        }.AsQueryable();
    }
}
