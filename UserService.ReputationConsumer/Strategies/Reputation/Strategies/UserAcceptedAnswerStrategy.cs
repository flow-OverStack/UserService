using UserService.Domain.Enums;
using UserService.ReputationConsumer.Strategies.Reputation.Base;

namespace UserService.ReputationConsumer.Strategies.Reputation.Strategies;

public class UserAcceptedAnswerStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.UserAcceptedAnswer;

    public int CalculateReputationChange()
    {
        return 2;
    }
}