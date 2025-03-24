using UserService.Domain.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class QuestionUpvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.QuestionUpvote;

    public int CalculateReputationChange()
    {
        return 10;
    }
}