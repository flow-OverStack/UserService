using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation;

public class ReputationStrategyResolver(IEnumerable<IReputationStrategy> strategies) : IReputationStrategyResolver
{
    public IReputationStrategy Resolve(string eventType)
    {
        return strategies.FirstOrDefault(s => s.EventType == eventType) ??
               throw new ArgumentException($"No strategy found for {eventType} event type.");
    }
}