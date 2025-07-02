using UserService.Domain.Enums;
using UserService.Messaging.Strategies.Reputation.Base;

namespace UserService.Messaging.Strategies.Reputation.Strategies;

public class QuestionDownvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.QuestionDownvote;

    public int CalculateReputationChange()
    {
        return -2;
    }
}