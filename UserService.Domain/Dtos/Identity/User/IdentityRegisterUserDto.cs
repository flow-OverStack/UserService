namespace UserService.Domain.Dtos.Identity.User;

public record IdentityRegisterUserDto(long Id, string Username, string Email, List<Entities.Role> Roles)
{
    public string Password { get; set; } //setter is here because mapper gets hashed password
}