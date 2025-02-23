using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dto.Requests.UserRole;
using UserService.Domain.Dto.Role;
using UserService.Domain.Dto.Token;
using UserService.Domain.Dto.User;
using UserService.Domain.Dto.UserRole;
using UserService.Domain.Entity;
using UserService.Domain.Resources;
using UserService.Domain.Result;
using UserService.Tests.Constants;
using UserService.Tests.Extensions;
using UserService.Tests.FunctionalTests.Base.Exception;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ExceptionTests : ExceptionBaseFunctionalTest
{
    public ExceptionTests(ExceptionFunctionalTestWebAppFactory factory) : base(factory)
    {
        var accessToken = TokenExtensions.GetRsaTokenWithRoleClaims("testuser1", [
            new Role { Name = "User" },
            new Role { Name = "Admin" }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RegisterUser_ShouldBe_Exception()
    {
        //Arrange
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Auth/register", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<TokenDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteRole_ShouldBe_Exception()
    {
        //Arrange
        const long roleId = 3;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/role/{roleId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRole_ShouldBe_Exception()
    {
        //Arrange
        var dto = new RoleDto(3, "UpdatedTestRole");

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/Role", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_Exception()
    {
        //Arrange
        const string username = "TestUser1";
        var dto = new RequestUserRoleDto
        {
            RoleId = 3
        };

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/role/" + username, dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_Exception()
    {
        //Arrange
        const string username = "TestUser2";
        const long roleId = 3;

        //Act
        var response =
            await HttpClient.DeleteAsync($"/api/v1.0/role/{username}/{roleId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);


        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_Exception()
    {
        //Arrange
        const string username = "TestUser2";
        var dto = new RequestUpdateUserRoleDto
        {
            FromRoleId = 3,
            ToRoleId = 2
        };

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/role/" + username, dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.StartsWith(ErrorMessage.InternalServerError, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}