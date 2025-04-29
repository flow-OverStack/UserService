using UserService.Domain.Interfaces.Entity;

namespace UserService.Domain.Entities;

public class User : IEntityId<long>, IAuditable
{
    public Guid KeycloakId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime LastLoginAt { get; set; } //is updated at successful login ot token update  
    public int Reputation { get; set; }

    public int ReputationEarnedToday { get; set; }

    public List<Role> Roles { get; set; }

    public DateTime CreatedAt { get; set; }
    public long Id { get; set; }

    //Questions, Badges, Answers and UserTags are not implemented here
    //They will be implemented via GraphQl via corresponding microservices
}