using UserService.Domain.Interfaces;

namespace UserService.Domain.Entity;

public class Role : IEntityId<long>
{
    public string Name { get; set; }

    public List<User> Users { get; set; }
    public long Id { get; set; }

    //Added for role mapping in keycloak service
    public override string ToString()
    {
        return Name;
    }
}