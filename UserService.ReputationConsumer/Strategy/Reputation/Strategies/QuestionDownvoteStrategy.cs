using UserService.Domain.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class QuestionDownvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.QuestionDownvote;

    public int CalculateReputationChange()
    {
        return -2;
    }
}