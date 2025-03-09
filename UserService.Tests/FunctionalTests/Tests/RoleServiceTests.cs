using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dto.Requests.UserRole;
using UserService.Domain.Dto.Role;
using UserService.Domain.Dto.UserRole;
using UserService.Domain.Entity;
using UserService.Domain.Resources;
using UserService.Domain.Result;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

[Collection("RoleSequentialTests")]
public class RoleServiceTests : SequentialFunctionalTest
{
    public RoleServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        var accessToken = TokenHelper.GetRsaTokenWithRoleClaims("testuser1", [
            new Role { Name = "User" },
            new Role { Name = "Admin" }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }


    [Trait("Category", "Functional")]
    [Fact]
    public async Task CreateRole_ShouldBe_Success()
    {
        //Arrange
        var dto = new CreateRoleDto("NewTestRole");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Role", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task CreateRole_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new CreateRoleDto("User");

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/Role", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleAlreadyExists);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteRole_ShouldBe_Success()
    {
        //Arrange
        const long roleId = 3;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/role/{roleId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteRole_ShouldBe_BadRequest()
    {
        //Arrange
        const long roleId = 0;

        //Act
        var response = await HttpClient.DeleteAsync($"/api/v1.0/role/{roleId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRole_ShouldBe_Success()
    {
        //Arrange
        var dto = new RoleDto(3, "UpdatedTestRole");

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/Role", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRole_ShouldBe_BadRequest()
    {
        //Arrange
        var dto = new RoleDto(0, "UpdatedTestRole");

        //Act
        var response = await HttpClient.PutAsJsonAsync("/api/v1.0/Role", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_Success()
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_BadRequest()
    {
        //Arrange
        const string username = "NotExistingUser";
        var dto = new RequestUserRoleDto
        {
            RoleId = 3
        };

        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1.0/role/" + username, dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.UserNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_Success()
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_BadRequest()
    {
        //Arrange
        const string username = "NotExistingUser";
        const long roleId = 3;

        //Act
        var response =
            await HttpClient.DeleteAsync($"/api/v1.0/role/{username}/{roleId}");
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.UserNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_Success()
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_BadRequest()
    {
        //Arrange
        const string username = "NotExistingUser";
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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.UserNotFound);
        Assert.Null(result.Data);
    }
}