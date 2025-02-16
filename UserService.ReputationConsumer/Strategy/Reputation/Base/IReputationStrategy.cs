using UserService.ReputationConsumer.Events;

namespace UserService.ReputationConsumer.Strategy.Reputation.Base;

public interface IReputationStrategy
{
    /// <summary>
    ///     The type of event
    /// </summary>
    string EventType { get; }

    /// <summary>
    ///     Calculates the change of reputation
    /// </summary>
    /// <param name="baseEvent"></param>
    /// <returns>Can be negative</returns>
    int CalculateReputationChange(BaseEvent baseEvent);
}