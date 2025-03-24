using UserService.Domain.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class AnswerUpvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.AnswerUpvote;

    public int CalculateReputationChange()
    {
        return 10;
    }
}