using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class AnswerUpvoteStrategy : IReputationStrategy
{
    public string EventType => "AnswerUpvote";

    public int CalculateReputationChange()
    {
        return 10;
    }
}