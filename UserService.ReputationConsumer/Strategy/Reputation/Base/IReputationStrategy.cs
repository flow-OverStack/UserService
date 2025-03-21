using UserService.ReputationConsumer.Enum;

namespace UserService.ReputationConsumer.Strategy.Reputation.Base;

public interface IReputationStrategy
{
    /// <summary>
    ///     The type of event
    /// </summary>
    BaseEventType EventType { get; }

    /// <summary>
    ///     Calculates the change of reputation
    /// </summary>
    /// <returns>Can be negative</returns>
    int CalculateReputationChange();
}