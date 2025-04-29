using UserService.Domain.Enums;
using UserService.ReputationConsumer.Strategies.Reputation.Base;

namespace UserService.ReputationConsumer.Strategies.Reputation.Strategies;

public class QuestionDownvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.QuestionDownvote;

    public int CalculateReputationChange()
    {
        return -2;
    }
}