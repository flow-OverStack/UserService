using UserService.Domain.Interfaces.Entity;

namespace UserService.Domain.Entities;

public class ReputationRecord : IEntityId<long>, IAuditable
{
    public long ReputationTargetId { get; set; }
    public User ReputationTarget { get; set; }
    public long InitiatorId { get; set; }
    public User Initiator { get; set; }
    public long ReputationRuleId { get; set; }
    public ReputationRule ReputationRule { get; set; }
    public long EntityId { get; set; }
    public bool Enabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public long Id { get; set; }
}