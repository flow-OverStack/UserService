using UserService.Domain.Enums;
using UserService.ReputationConsumer.Strategies.Reputation.Base;

namespace UserService.ReputationConsumer.Strategies.Reputation.Strategies;

public class AnswerDownvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.AnswerDownvote;

    public int CalculateReputationChange()
    {
        return -2;
    }
}