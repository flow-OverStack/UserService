namespace UserService.ReputationConsumer.Events;

public class BaseEvent
{
    public string EventType { get; set; }

    public long UserId { get; set; }
}