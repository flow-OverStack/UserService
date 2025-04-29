using UserService.Domain.Interfaces.Entity;
using UserService.Domain.Interfaces.Entity.Role;

namespace UserService.Domain.Entities;

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