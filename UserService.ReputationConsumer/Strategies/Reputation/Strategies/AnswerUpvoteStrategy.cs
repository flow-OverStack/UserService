using UserService.Domain.Enums;
using UserService.ReputationConsumer.Strategies.Reputation.Base;

namespace UserService.ReputationConsumer.Strategies.Reputation.Strategies;

public class AnswerUpvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.AnswerUpvote;

    public int CalculateReputationChange()
    {
        return 10;
    }
}