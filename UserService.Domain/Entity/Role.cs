using UserService.Domain.Interfaces.Entities;
using UserService.Domain.Interfaces.Entities.Role;

namespace UserService.Domain.Entity;

public class Role : IEntityId<long>, IRoleNameProvider
{
    public string Name { get; set; }

    public List<User> Users { get; set; }
    public long Id { get; set; }

    //Added for role mapping in keycloak service
    public string GetRoleName()
    {
        return Name;
    }
}