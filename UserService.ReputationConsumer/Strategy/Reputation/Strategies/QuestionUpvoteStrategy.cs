using UserService.ReputationConsumer.Events;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation.Strategies;

public class QuestionUpvoteStrategy : IReputationStrategy
{
    public string EventType => "QuestionUpvote";

    public int CalculateReputationChange(BaseEvent baseEvent)
    {
        return 10;
    }
}