using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dtos.Request.Role;
using UserService.Domain.Dtos.Request.UserRole;
using UserService.Domain.Dtos.Role;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Dtos.User;
using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Entities;
using UserService.Domain.Resources;
using UserService.Domain.Results;
using UserService.Tests.Constants;
using UserService.Tests.FunctionalTests.Base.Exception;
using UserService.Tests.FunctionalTests.Configurations.GraphQl;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ExceptionTests : ExceptionBaseFunctionalTest
{
    public ExceptionTests(ExceptionFunctionalTestWebAppFactory factory) : base(factory)
    {
        var accessToken = TokenHelper.GetRsaTokenWithRoleClaims("testuser1", [
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
        const long roleId = 3;
        var dto = new RequestRoleDto("UpdatedTestRole");

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/Role/{roleId}", dto);
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
        const long roleId = 3;

        //Act
        var response = await HttpClient.PostAsync($"/api/v1.0/role/{username}/{roleId}", null);
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

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetQuestionById_ShouldBe_NoException()
    {
        //Arrange
        var requestBody = new { query = GraphQlHelper.RequestUserByIdQuery(1) };

        //Act
        // 1st request fetches data from DB
        await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        // 2nd request fetches data from cache
        var response = await HttpClient.PostAsJsonAsync(GraphQlHelper.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlGetUserByIdResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.User);
        Assert.NotNull(result.Data.User.Roles);
    }
}