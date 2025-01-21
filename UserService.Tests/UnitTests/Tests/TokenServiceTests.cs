using UserService.Domain.Dto.Token;
using UserService.Domain.Resources;
using UserService.Tests.Extensions;
using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class TokenServiceTests
{
    public static IEnumerable<object[]> GetRefreshTokenData()
    {
        return
        [
            [SigningKeyExtensions.GetRsaToken(), true],
            [SigningKeyExtensions.GetHmacToken(), false]
        ];
    }

    [Trait("Category", "Unit")]
    [Theory]
    [MemberData(nameof(GetRefreshTokenData))]
    public async Task RefreshToken_ShouldBe_NewToken_Or_InvalidToken(string accessToken, bool isTokenValid)
    {
        //Arrange
        await Task.Delay(TimeSpan.FromSeconds(1)); //wait for tokens to expire

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
}