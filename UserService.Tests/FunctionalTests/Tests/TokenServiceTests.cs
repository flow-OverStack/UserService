using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dto.Token;
using UserService.Domain.Resources;
using UserService.Domain.Result;
using UserService.Tests.Extensions;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class TokenServiceTests : BaseFunctionalTest
{
    public TokenServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    public static IEnumerable<object[]> GetAccessTokensForInvalidUser()
    {
        return
        [
            [SigningKeyExtensions.GetRsaToken("testuser2"), "TestRefreshToken2"],
            [SigningKeyExtensions.GetRsaToken("testuser1"), "WrongRefreshToken1"]
        ];
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RefreshToken_ShouldBe_NewToken()
    {
        //Arrange
        var dto = new RefreshTokenDto
        {
            AccessToken = SigningKeyExtensions.GetRsaToken("testuser1"),
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

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RefreshToken_ShouldBe_InvalidToken()
    {
        //Arrange
        var dto = new RefreshTokenDto
        {
            AccessToken = SigningKeyExtensions.GetHmacToken("testuser1"),
            RefreshToken = "TestRefreshToken1"
        };

        //Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/token/refresh", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.InvalidToken);
        Assert.Null(result.Data);
    }

    [Theory]
    [Trait("Category", "Functional")]
    [MemberData(nameof(GetAccessTokensForInvalidUser))]
    public async Task RefreshToken_ShouldBe_InvalidClientRequest_When_RefreshToken_IsNull_Or_IsExpired(
        string accessToken,
        string refreshToken)
    {
        //Arrange
        var dto = new RefreshTokenDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        //Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/token/refresh", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.InvalidClientRequest);
        Assert.Null(result.Data);
    }
}