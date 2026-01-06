using UserService.Domain.Enums;
using UserService.Domain.Interfaces.Entity;

namespace UserService.Domain.Entities;

public class ReputationRule : IEntityId<long>
{
    public string EventType { get; set; }
    public string EntityType { get; set; }
    public string? Group { get; set; }
    public int ReputationChange { get; set; }
    public List<ReputationRecord> ReputationRecords { get; set; }

    // ReputationTarget is a rule application parameter (policy), not a domain concept with identity or lifecycle, so a separate table is unnecessary.
    public ReputationTarget ReputationTarget { get; set; }
    public long Id { get; set; }
}