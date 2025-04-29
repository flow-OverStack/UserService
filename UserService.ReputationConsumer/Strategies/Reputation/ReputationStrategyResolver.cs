using UserService.Domain.Enums;
using UserService.ReputationConsumer.Strategies.Reputation.Base;

namespace UserService.ReputationConsumer.Strategies.Reputation;

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