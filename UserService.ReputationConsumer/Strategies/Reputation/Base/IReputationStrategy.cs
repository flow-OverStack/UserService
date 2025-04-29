using UserService.Domain.Enums;

namespace UserService.ReputationConsumer.Strategies.Reputation.Base;

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