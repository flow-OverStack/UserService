using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Results;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Base;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class TokenServiceTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    public async Task RefreshToken_ValidRefreshToken_ReturnsOk()
    {
        //Arrange
        var dto = new RefreshTokenDto("TestRefreshToken1");

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
    public async Task RefreshToken_InvalidRefreshToken_ReturnsBadRequest()
    {
        //Arrange
        var dto = new RefreshTokenDto(TestConstants.WrongRefreshToken);

        //Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/token/refresh", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidToken, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}