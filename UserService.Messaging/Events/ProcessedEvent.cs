namespace UserService.Messaging.Events;

public class ProcessedEvent
{
    public Guid EventId { get; set; }
    public DateTime ProcessedAt { get; set; }
}