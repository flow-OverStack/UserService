using UserService.Domain.Interfaces.Entity;

namespace UserService.Domain.Entities;

public class ReputationRule : IEntityId<long>
{
    public string EventType { get; set; }
    public string EntityType { get; set; }
    public string? Group { get; set; }
    public int ReputationChange { get; set; }
    public List<ReputationRecord> ReputationRecords { get; set; }
    public long Id { get; set; }
}