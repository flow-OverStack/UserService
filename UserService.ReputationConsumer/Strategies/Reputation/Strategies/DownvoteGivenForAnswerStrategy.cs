using UserService.Domain.Enums;
using UserService.ReputationConsumer.Strategies.Reputation.Base;

namespace UserService.ReputationConsumer.Strategies.Reputation.Strategies;

public class DownvoteGivenForAnswerStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.DownvoteGivenForAnswer;

    public int CalculateReputationChange()
    {
        return -1;
    }
}