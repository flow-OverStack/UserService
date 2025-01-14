using UserService.Domain.Interfaces;

namespace UserService.Domain.Entity;

public class User : IEntityId<long>, IAuditable
{
    public Guid KeycloakId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime LastLoginAt { get; set; } //is updated at successful login ot token update  

    public UserToken UserToken { get; set; }
    public int Reputation { get; set; }

    public List<Role> Roles { get; set; }
    public List<Badge> Badges { get; set; }

    public DateTime CreatedAt { get; set; }
    public long Id { get; set; }

    //Questions, Answers and UserTags are not implemented here
    //They will be implemented via GraphQl via corresponding microservices
}