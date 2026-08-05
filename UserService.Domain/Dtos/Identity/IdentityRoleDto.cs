using UserService.Domain.Interfaces.Entity.Role;

namespace UserService.Domain.Dtos.Identity;

public record IdentityRoleDto(string Name) : INameProvider;
