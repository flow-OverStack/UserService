namespace UserService.ReputationConsumer.Events;

public class ProcessedEvent
{
    public Guid EventId { get; set; }
    public DateTime ProcessedAt { get; set; }
}