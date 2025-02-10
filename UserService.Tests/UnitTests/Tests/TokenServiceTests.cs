using UserService.Domain.Dto.Token;
using UserService.Domain.Resources;
using UserService.Tests.Extensions;
using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class TokenServiceTests
{
    public static IEnumerable<object[]> GetAccessTokensForInvalidUser()
    {
        return
        [
            [TokenExtensions.GetRsaToken("testuser2"), "TestRefreshToken2"],
            [TokenExtensions.GetRsaToken("testuser1"), "WrongRefreshToken1"]
        ];
    }


    [Trait("Category", "Unit")]
    [Fact]
    public async Task RefreshToken_ShouldBe_NewToken()
    {
        //Arrange
        var accessToken = TokenExtensions.GetRsaToken("testuser1");
        var tokenService = new TokenServiceFactory().GetService();
        var dto = new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "TestRefreshToken1"
        };

        //Act
        var result = await tokenService.RefreshToken(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RefreshToken_ShouldBe_InvalidToken()
    {
        //Arrange
        var accessToken = TokenExtensions.GetHmacToken("testuser1");
        var tokenService = new TokenServiceFactory().GetService();
        var dto = new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "TestRefreshToken1"
        };

        //Act
        var result = await tokenService.RefreshToken(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.StartsWith(result.ErrorMessage, ErrorMessage.InvalidToken);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Theory]
    [MemberData(nameof(GetAccessTokensForInvalidUser))]
    public async Task RefreshToken_ShouldBe_InvalidClientRequest_When_UserToken_IsNull_Or_IsExpired(string accessToken,
        string refreshToken)
    {
        //Arrange
        var tokenService = new TokenServiceFactory().GetService();
        var dto = new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        //Act
        var result = await tokenService.RefreshToken(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidClientRequest, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}