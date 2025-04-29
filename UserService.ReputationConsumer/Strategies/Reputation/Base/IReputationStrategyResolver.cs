namespace UserService.ReputationConsumer.Strategies.Reputation.Base;

public interface IReputationStrategyResolver
{
    IReputationStrategy Resolve(string eventType);
}