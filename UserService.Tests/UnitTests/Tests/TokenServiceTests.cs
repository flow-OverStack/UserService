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
            [SigningKeyExtensions.GetRsaToken("TestUser2"), "TestRefreshToken2"],
            [SigningKeyExtensions.GetRsaToken("TestUser3"), "WrongRefreshToken2"]
        ];
    }


    [Trait("Category", "Unit")]
    [Fact]
    public async Task RefreshToken_ShouldBe_NewToken()
    {
        //Arrange
        var accessToken = SigningKeyExtensions.GetRsaToken("TestUser1");

        var tokenService = new TokenServiceFactory().GetService();

        //Act
        var result = await tokenService.RefreshToken(new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "TestRefreshToken1"
        });

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RefreshToken_ShouldBe_InvalidToken()
    {
        //Arrange
        var accessToken = SigningKeyExtensions.GetHmacToken("TestUser1");

        var tokenService = new TokenServiceFactory().GetService();

        //Act
        var result = await tokenService.RefreshToken(new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "TestRefreshToken1"
        });

        //Assert
        Assert.False(result.IsSuccess);
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

        //Act
        var result = await tokenService.RefreshToken(new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidClientRequest, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}