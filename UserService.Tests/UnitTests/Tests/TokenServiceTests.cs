using UserService.Domain.Dto.Token;
using UserService.Domain.Resources;
using UserService.Tests.Extensions;
using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class TokenServiceTests
{
    public static IEnumerable<object[]> GetAccessTokensForValidUser()
    {
        return
        [
            [SigningKeyExtensions.GetRsaToken("TestUser1").GetAwaiter().GetResult(), true],
            [SigningKeyExtensions.GetHmacToken("TestUser1").GetAwaiter().GetResult(), false]
        ];
    }

    public static IEnumerable<object[]> GetAccessTokensForInvalidUser()
    {
        return
        [
            [SigningKeyExtensions.GetRsaToken("TestUser2").GetAwaiter().GetResult(), "TestRefreshToken2"],
            [SigningKeyExtensions.GetRsaToken("TestUser3").GetAwaiter().GetResult(), "WrongRefreshToken2"]
        ];
    }


    [Trait("Category", "Unit")]
    [Theory]
    [MemberData(nameof(GetAccessTokensForValidUser))]
    public async Task RefreshToken_ShouldBe_NewToken_Or_InvalidToken(string accessToken, bool isTokenValid)
    {
        //Arrange
        var tokenService = new TokenServiceFactory().GetService();

        //Act
        var result = await tokenService.RefreshToken(new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = "TestRefreshToken1"
        });

        //Assert
        if (isTokenValid)
        {
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
        }
        else
        {
            Assert.False(result.IsSuccess);
            Assert.StartsWith(ErrorMessage.InvalidToken, result.ErrorMessage);
            Assert.Null(result.Data);
        }
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