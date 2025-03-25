namespace UserService.Domain.Events;

public class BaseEvent
{
    public Guid EventId { get; set; }

    public string EventType { get; set; }

    public long UserId { get; set; }
}