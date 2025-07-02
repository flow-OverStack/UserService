using UserService.Domain.Enums;
using UserService.Messaging.Strategies.Reputation.Base;

namespace UserService.Messaging.Strategies.Reputation.Strategies;

public class AnswerUpvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.AnswerUpvote;

    public int CalculateReputationChange()
    {
        return 10;
    }
}