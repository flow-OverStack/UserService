using UserService.Domain.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class DownvoteGivenForAnswerStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.DownvoteGivenForAnswer;

    public int CalculateReputationChange()
    {
        return -1;
    }
}