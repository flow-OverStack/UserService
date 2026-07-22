using UserService.Domain.Dtos.Token;
using UserService.Tests.UnitTests.Factories;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class TokenServiceTests
{
    [Fact]
    public async Task RefreshTokenAsync_ValidRefreshToken_ReturnsNewToken()
    {
        //Arrange
        var tokenService = new TokenServiceFactory().GetService();
        var dto = new RefreshTokenDto("TestRefreshToken1");

        //Act
        var result = await tokenService.RefreshTokenAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}