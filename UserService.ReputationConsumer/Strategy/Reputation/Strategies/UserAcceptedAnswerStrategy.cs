using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class UserAcceptedAnswerStrategy : IReputationStrategy
{
    public string EventType => "UserAcceptedAnswer";

    public int CalculateReputationChange()
    {
        return 2;
    }
}