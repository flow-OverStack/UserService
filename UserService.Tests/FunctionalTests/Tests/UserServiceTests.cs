using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Api.Dtos.User;
using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Domain.Results;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class UserServiceTests(FunctionalTestWebAppFactory factory) : SequentialFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateMyUsername_ShouldBe_Ok()
    {
        //Arrange
        var accessToken = TokenHelper.GetRsaToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var dto = new RequestUpdateUsernameDto("newusername");

        //Act
        var response = await HttpClient.PatchAsJsonAsync("/api/v1/user/me/username", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("newusername", result.Data.Username);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateMyUsername_ShouldBe_BadRequest()
    {
        //Arrange
        var accessToken = TokenHelper.GetRsaToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var dto = new RequestUpdateUsernameDto("invalid!name");

        //Act
        var response = await HttpClient.PatchAsJsonAsync("/api/v1/user/me/username", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidUsername, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateUsernameById_ShouldBe_Ok()
    {
        //Arrange
        var accessToken = TokenHelper.GetRsaToken(roles: [new Role { Name = "Admin" }]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        const long targetUserId = 3;
        var dto = new RequestUpdateUsernameDto("updatedusername");

        //Act
        var response = await HttpClient.PatchAsJsonAsync($"/api/v1/user/{targetUserId}/username", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("updatedusername", result.Data.Username);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateUsernameById_ShouldBe_BadRequest()
    {
        //Arrange
        var accessToken = TokenHelper.GetRsaToken(roles: [new Role { Name = "Admin" }]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        const long targetUserId = 3;
        var dto = new RequestUpdateUsernameDto("invalid!name");

        //Act
        var response = await HttpClient.PatchAsJsonAsync($"/api/v1/user/{targetUserId}/username", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(result!.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidUsername, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateUsernameById_ShouldBe_Unauthorized()
    {
        //Arrange
        const long targetUserId = 3;
        var dto = new RequestUpdateUsernameDto("newusername");

        //Act
        var response = await HttpClient.PatchAsJsonAsync($"/api/v1/user/{targetUserId}/username", dto);

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task UpdateUsernameById_ShouldBe_Forbidden()
    {
        //Arrange
        var accessToken = TokenHelper.GetRsaToken();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        const long targetUserId = 3;
        var dto = new RequestUpdateUsernameDto("newusername");

        //Act
        var response = await HttpClient.PatchAsJsonAsync($"/api/v1/user/{targetUserId}/username", dto);

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}