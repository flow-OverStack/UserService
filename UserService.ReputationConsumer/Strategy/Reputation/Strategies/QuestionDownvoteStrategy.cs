using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class QuestionDownvoteStrategy : IReputationStrategy
{
    public string EventType => "QuestionDownvote";

    public int CalculateReputationChange()
    {
        return -2;
    }
}