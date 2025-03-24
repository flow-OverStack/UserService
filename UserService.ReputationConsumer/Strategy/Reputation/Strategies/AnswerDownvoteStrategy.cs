using UserService.Domain.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class AnswerDownvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.AnswerDownvote;

    public int CalculateReputationChange()
    {
        return -2;
    }
}