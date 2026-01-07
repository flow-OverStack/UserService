using UserService.Domain.Interfaces.Entity;

namespace UserService.Domain.Entities;

public class User : IEntityId<long>, IAuditable
{
    public string IdentityId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime LastLoginAt { get; set; }
    public List<ReputationRecord> OwnedReputationRecords { get; set; }
    public List<ReputationRecord> InitiatedReputationRecords { get; set; }
    public List<Role> Roles { get; set; }
    public DateTime CreatedAt { get; set; }
    public long Id { get; set; }
}