using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Domain.Dto.User;
using UserService.Domain.Result;
using UserService.Tests.Extensions;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class AuthServiceTests : BaseFunctionalTest
{
    public AuthServiceTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RegisterUser_ShouldBe_Success()
    {
        //Arrange
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4", TestConstants.TestPassword + "4");
        //Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/auth/register", dto);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<BaseResult<UserDto>>(body);

        //Assert
        Assert.True(result!.IsSuccess);
        Assert.NotNull(result.Data);
    }
}