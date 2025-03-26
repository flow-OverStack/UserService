using UserService.Domain.Enum;
using UserService.ReputationConsumer.Strategy.Reputation.Base;

namespace UserService.ReputationConsumer.Strategy.Reputation;

public class ReputationStrategyResolver(IEnumerable<IReputationStrategy> strategies) : IReputationStrategyResolver
{
    public IReputationStrategy Resolve(string eventType)
    {
        var notFoundException = new InvalidOperationException($"No strategy found for {eventType} event type.");

        if (!Enum.TryParse<BaseEventType>(eventType, out var baseEventType))
            throw notFoundException;

        return strategies.FirstOrDefault(s => s.EventType == baseEventType) ??
               throw notFoundException;
    }
}