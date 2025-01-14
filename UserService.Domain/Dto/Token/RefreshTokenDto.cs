namespace UserService.Domain.Dto.Token;

public class RefreshTokenDto
{
    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }
}