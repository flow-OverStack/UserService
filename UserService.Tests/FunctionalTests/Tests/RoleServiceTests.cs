using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dto.Role;
using UserService.Domain.Dto.User;
using UserService.Domain.Entity;
using UserService.Domain.Result;
using UserService.Tests.Extensions;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class RoleServiceTests : BaseFunctionalTest
{
    public RoleServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        var accessToken = TokenExtensions.GetRsaTokenWithRoleClaims("testuser1", [
            new Role { Name = "User" }, new Role { Name = "Admin" }
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
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }
}