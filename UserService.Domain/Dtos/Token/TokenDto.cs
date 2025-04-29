namespace UserService.Domain.Dtos.Token;

public class TokenDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public DateTime AccessExpires { get; set; }

    public DateTime RefreshExpires { get; set; }
}