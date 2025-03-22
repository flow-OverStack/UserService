using UserService.ReputationConsumer.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class UserAcceptedAnswerStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.UserAcceptedAnswer;

    public int CalculateReputationChange()
    {
        return 2;
    }
}