using UserService.Domain.Enums;
using UserService.Messaging.Strategies.Reputation.Base;

namespace UserService.Messaging.Strategies.Reputation.Strategies;

public class QuestionUpvoteStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.QuestionUpvote;

    public int CalculateReputationChange()
    {
        return 10;
    }
}