namespace UserService.Domain.Dto.Token;

public class TokenDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public DateTime Expires { get; set; }
    public long UserId { get; set; }
}