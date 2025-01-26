using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dto.Token;
using UserService.Domain.Result;
using UserService.Tests.Extensions;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class TokenServiceTests : BaseFunctionalTest
{
    public TokenServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RefreshToken_ShouldBe_NewToken()
    {
        //Arrange
        var dto = new RefreshTokenDto
        {
            AccessToken = SigningKeyExtensions.GetRsaToken("TestUser1"),
            RefreshToken = "TestRefreshToken1"
        };

        //Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/token/refresh", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }
}