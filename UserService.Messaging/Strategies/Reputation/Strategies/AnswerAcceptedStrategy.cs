using UserService.Domain.Enums;
using UserService.Messaging.Strategies.Reputation.Base;

namespace UserService.Messaging.Strategies.Reputation.Strategies;

public class AnswerAcceptedStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.AnswerAccepted;

    public int CalculateReputationChange()
    {
        return 15;
    }
}