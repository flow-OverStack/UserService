using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dto.Token;
using UserService.Domain.Dto.User;
using UserService.Domain.Resources;
using UserService.Domain.Result;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class AuthServiceTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RegisterUser_ShouldBe_Success()
    {
        //Arrange
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/auth/register", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
        Assert.Equal(ErrorMessage.EmailNotValid, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoginUserWithUsername_ShouldBe_Success()
    {
        //Arrange
        var dto = new LoginUsernameUserDto("TestUser3",
            TestConstants.TestPassword + "3");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login-username", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoginUserWithEmail_ShouldBe_Success()
    {
        //Arrange
        var dto = new LoginEmailUserDto("TestUser1@test.com",
            TestConstants.TestPassword + "1");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login-email", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoginUserWithEmail_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new LoginEmailUserDto("NotEmail", TestConstants.TestPassword + "1");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login-email", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.EmailNotValid, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoginUserWithUsername_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new LoginUsernameUserDto("TestUser1",
            TestConstants.WrongPassword);

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login-username", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.PasswordIsWrong, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}