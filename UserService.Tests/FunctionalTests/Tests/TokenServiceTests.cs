using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dto.Token;
using UserService.Domain.Resources;
using UserService.Domain.Result;
using UserService.Tests.Extensions;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class TokenServiceTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    [Trait("Category", "Functional")]
    public async Task RefreshToken_ShouldBe_Success()
    {
        //Arrange
        var dto = new RefreshTokenDto
        {
            AccessToken = TokenExtensions.GetRsaToken("testuser1"),
            RefreshToken = "TestRefreshToken1"
        };

        //Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/token/refresh", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RefreshToken_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new RefreshTokenDto
        {
            AccessToken = TokenExtensions.GetHmacToken("testuser1"),
            RefreshToken = "TestRefreshToken1"
        };

        //Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/token/refresh", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.InvalidToken);
        Assert.Null(result.Data);
    }
}