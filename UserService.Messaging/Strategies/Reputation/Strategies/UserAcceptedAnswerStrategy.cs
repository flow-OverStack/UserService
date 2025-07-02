using UserService.Domain.Enums;
using UserService.Messaging.Strategies.Reputation.Base;

namespace UserService.Messaging.Strategies.Reputation.Strategies;

public class UserAcceptedAnswerStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.UserAcceptedAnswer;

    public int CalculateReputationChange()
    {
        return 2;
    }
}