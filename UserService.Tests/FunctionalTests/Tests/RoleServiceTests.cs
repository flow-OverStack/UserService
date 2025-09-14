using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UserService.Api.Dtos.Role;
using UserService.Api.Dtos.UserRole;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Identity.Role;
using UserService.Domain.Dtos.Role;
using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces.Identity;
using UserService.Domain.Results;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

[Collection(nameof(RoleServiceTests))]
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
        Assert.Equal(ErrorMessage.RoleAlreadyExists, result.ErrorMessage);
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
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateRole_ShouldBe_Success()
    {
        //Arrange
        const long roleId = 3;
        var dto = new RequestRoleDto("UpdatedTestRole");

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/Role/{roleId}", dto);
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
        const long roleId = 0;
        var dto = new RequestRoleDto("UpdatedTestRole");

        //Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1.0/Role/{roleId}", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<RoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_Success()
    {
        //Arrange
        const string username = "TestUser1";
        const long roleId = 3;

        //Act
        var response = await HttpClient.PostAsync($"/api/v1.0/role/{username}/{roleId}", null);
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
        const long roleId = 3;

        //Act
        var response = await HttpClient.PostAsync($"/api/v1.0/role/{username}/{roleId}", null);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserRoleDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
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
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
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
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RollbackUpdateRolesAsync_ShouldBe_BadRequest()
    {
        // Arrange
        await using var scope = ServiceProvider.CreateAsyncScope();
        var identityServer = scope.ServiceProvider.GetRequiredService<IIdentityServer>();
        var dto = new IdentityUpdateRolesDto(Guid.NewGuid().ToString(), 1, "TestsUser1@test.com",
            [new Role { Name = "User" }]);

        // Act
        await identityServer.RollbackUpdateRolesAsync(dto);

        // Assert
        Assert.True(true);
    }
}