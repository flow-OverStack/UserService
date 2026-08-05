namespace UserService.Domain.Dtos.Identity;

public record IdentityRegisterUserDto(long Id, string Username, string Email, List<IdentityRoleDto> Roles)
{
    public string Password { get; set; }
}