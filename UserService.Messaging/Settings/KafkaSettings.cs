namespace UserService.Messaging.Settings;

public class KafkaSettings
{
    public string Host { get; set; }
    public string ReputationTopic { get; set; }
    public string ReputationConsumerGroup { get; set; }
    public string DeadLetterTopic { get; set; }
}