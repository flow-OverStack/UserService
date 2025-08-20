namespace UserService.Messaging.Messages;

public class FaultedMessage
{
    public string SerializedMessage { get; set; }
    public string ErrorMessage { get; set; }
    public string StackTrace { get; set; }
    public string Source { get; set; }
}