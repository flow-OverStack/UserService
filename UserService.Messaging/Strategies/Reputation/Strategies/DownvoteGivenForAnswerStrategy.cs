using UserService.Domain.Enums;
using UserService.Messaging.Strategies.Reputation.Base;

namespace UserService.Messaging.Strategies.Reputation.Strategies;

public class DownvoteGivenForAnswerStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.DownvoteGivenForAnswer;

    public int CalculateReputationChange()
    {
        return -1;
    }
}