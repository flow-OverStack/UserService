using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UserService.Application.Resources;
using UserService.DAL;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Results;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.FunctionalTests.Tests;

[FunctionalTest]
public class AuthServiceTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Fact]
    public async Task RegisterUser_ValidData_ReturnsCreated()
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

    [Fact]
    public async Task RegisterUser_InvalidEmail_ReturnsBadRequest()
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

    [Fact]
    public async Task InitUser_ValidClaims_ReturnsOk()
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

    [Fact]
    public async Task InitUser_InvalidEmail_ReturnsBadRequest()
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

    [Fact]
    public async Task InitUser_EmptyEmailClaim_ReturnsForbidden()
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

    [Fact]
    public async Task LoginUser_ValidUsername_ReturnsOk()
    {
        //Arrange
        var dto = new LoginUserDto("TestUser3", TestConstants.TestPassword + "3");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task LoginUser_ValidEmail_ReturnsOk()
    {
        //Arrange
        var dto = new LoginUserDto("TestUser1@test.com", TestConstants.TestPassword + "1");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task LoginUser_NewKeycloakUser_ReturnsOkAndCreatesUser()
    {
        //Arrange
        var dto = new LoginUserDto(TestConstants.ExistingUsername,
            TestConstants.TestPassword + TestConstants.ExistingUsername);
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var intialCount = await dbContext.Set<User>().AsNoTracking().CountAsync();

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/login", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        var finalCount = await dbContext.Set<User>().AsNoTracking().CountAsync();
        Assert.Equal(intialCount + 1, finalCount); //New user should be created
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task LoginUser_WrongPassword_ReturnsUnauthorized()
    {
        //Arrange
        var dto = new LoginUserDto("TestUser1", TestConstants.WrongPassword);

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

    [Fact]
    public async Task LoginUser_NonExistentUser_ReturnsUnauthorized()
    {
        //Arrange
        var dto = new LoginUserDto("NonExistentUser", TestConstants.TestPassword);

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