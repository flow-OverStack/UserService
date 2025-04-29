using UserService.Domain.Enums;
using UserService.ReputationConsumer.Strategies.Reputation.Base;

namespace UserService.ReputationConsumer.Strategies.Reputation.Strategies;

public class AnswerAcceptedStrategy : IReputationStrategy
{
    public BaseEventType EventType => BaseEventType.AnswerAccepted;

    public int CalculateReputationChange()
    {
        return 15;
    }
}