using UserService.ReputationConsumer.Events;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class AnswerAcceptedStrategy : IReputationStrategy
{
    public string EventType => "AnswerAccepted";

    public int CalculateReputationChange(BaseEvent baseEvent)
    {
        return 15;
    }
}