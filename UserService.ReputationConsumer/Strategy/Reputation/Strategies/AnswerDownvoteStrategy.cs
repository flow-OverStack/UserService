using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class AnswerDownvoteStrategy : IReputationStrategy
{
    public string EventType => "AnswerDownvote";

    public int CalculateReputationChange()
    {
        return -2;
    }
}