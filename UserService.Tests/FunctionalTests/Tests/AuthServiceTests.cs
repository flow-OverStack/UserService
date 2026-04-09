using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Results;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class AuthServiceTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RegisterUser_ShouldBe_Created()
    {
        //Arrange
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/auth/register", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RegisterUser_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new RegisterUserDto("TestUser1", "NotEmail", TestConstants.TestPassword);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/auth/register", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidEmail, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task InitUser_ShouldBe_Ok()
    {
        //Arrange
        var accessToken =
            TokenHelper.GetRsaToken(username: "TestUser4", email: "TestsUser4@test.com",
                identityId: "test-identity-id-4");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //Act
        var response = await HttpClient.PostAsync("/api/v1/auth/init", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task InitUser_ShouldBe_BadRequest()
    {
        //Arrange
        var accessToken =
            TokenHelper.GetRsaToken(username: "testuser1", email: "NotEmail", identityId: "test-identity-id-1");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //Act
        var response = await HttpClient.PostAsync("/api/v1/auth/init", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidEmail, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task InitUser_ShouldBe_Unauthorized()
    {
        //Arrange
        var accessToken = TokenHelper.GetRsaToken(username: "testuser1", email: "", identityId: "test-identity-id-1");
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //Act
        var response = await HttpClient.PostAsync("/api/v1/auth/init", null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Invalid claims", body);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoginUserWithUsername_ShouldBe_Ok()
    {
        //Arrange
        var dto = new LoginUserDto("TestUser3",
            TestConstants.TestPassword + "3");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoginUserWithEmail_ShouldBe_Ok()
    {
        //Arrange
        var dto = new LoginUserDto("TestUser1@test.com",
            TestConstants.TestPassword + "1");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoginUserWithUsername_ShouldBe_Unauthorized()
    {
        //Arrange
        var dto = new LoginUserDto("TestUser1",
            TestConstants.WrongPassword);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidCredentials, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}