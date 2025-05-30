using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Resources;
using UserService.Domain.Results;
using UserService.Tests.Constants;
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
            RefreshToken = TestConstants.WrongRefreshToken
        };

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