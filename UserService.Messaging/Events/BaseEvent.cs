namespace UserService.Messaging.Events;

public class BaseEvent
{
    public Guid EventId { get; set; }

    public string EventType { get; set; }

    public long UserId { get; set; }

    public string? CancelsEvent { get; set; }
}