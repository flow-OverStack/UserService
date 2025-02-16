namespace UserService.ReputationConsumer.Strategy.Reputation.Base;

public interface IReputationStrategyResolver
{
    IReputationStrategy Resolve(string eventType);
}