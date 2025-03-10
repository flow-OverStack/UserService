using UserService.Domain.Dto.Token;
using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class TokenServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task RefreshToken_ShouldBe_NewToken()
    {
        //Arrange
        var tokenService = new TokenServiceFactory().GetService();
        var dto = new RefreshTokenDto
        {
            RefreshToken = "TestRefreshToken1"
        };

        //Act
        var result = await tokenService.RefreshToken(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}