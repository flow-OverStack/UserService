namespace UserService.Messaging.Strategies.Reputation.Base;

public interface IReputationStrategyResolver
{
    IReputationStrategy Resolve(string eventType);
}