using UserService.ReputationConsumer.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class AnswerAcceptedStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.AnswerAccepted;

    public int CalculateReputationChange()
    {
        return 15;
    }
}