namespace UserService.Messaging.Events;

public class BaseEvent
{
    public Guid EventId { get; set; }

    public string EventType { get; set; }

    public long AuthorId { get; set; }

    public long InitiatorId { get; set; }

    public string EntityType { get; set; }

    public long EntityId { get; set; }
}