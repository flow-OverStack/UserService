namespace UserService.Messaging.Settings;

public class KafkaSettings
{
    public string Host { get; set; }
    public string BaseEventTopic { get; set; }
    public string BaseEventConsumerGroup { get; set; }
    public string DeadLetterTopic { get; set; }
}