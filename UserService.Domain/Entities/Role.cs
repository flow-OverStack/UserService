using UserService.Domain.Interfaces.Entity;
using UserService.Domain.Interfaces.Entity.Role;

namespace UserService.Domain.Entities;

public class Role : IEntityId<long>, INameProvider
{
    public List<User> Users { get; set; }
    public long Id { get; set; }
    public string Name { get; set; }
}