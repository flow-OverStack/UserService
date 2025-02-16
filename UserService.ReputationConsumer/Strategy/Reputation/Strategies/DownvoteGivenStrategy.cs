using UserService.ReputationConsumer.Events;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class DownvoteGivenStrategy : IReputationStrategy
{
    public string EventType => "DownvoteGiven";

    public int CalculateReputationChange(BaseEvent baseEvent)
    {
        return -1;
    }
}