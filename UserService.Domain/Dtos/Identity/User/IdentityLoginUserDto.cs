namespace UserService.Domain.Dtos.Identity.User;

public record IdentityLoginUserDto(string Username)
{
    public string Password { get; set; }
}