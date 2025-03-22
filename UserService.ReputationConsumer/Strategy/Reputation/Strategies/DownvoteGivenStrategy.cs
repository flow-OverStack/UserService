using UserService.ReputationConsumer.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class DownvoteGivenStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.DownvoteGiven;

    public int CalculateReputationChange()
    {
        return -1;
    }
}