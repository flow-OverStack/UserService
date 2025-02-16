using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class QuestionUpvoteStrategy : IReputationStrategy
{
    public string EventType => "QuestionUpvote";

    public int CalculateReputationChange()
    {
        return 10;
    }
}