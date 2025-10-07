using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class UserActivityServiceTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Fact]
    [Trait("Category", "Functional")]
    public async Task RegisterHeartbeat_ShouldBe_Ok()
    {
        //Arrange
        var accessToken = TokenHelper.GetRsaTokenWithUserId(1);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //Act
        var response = await HttpClient.PostAsync("api/v1.0/UserActivity/heartbeat", null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(string.IsNullOrEmpty(body));
    }

    [Fact]
    [Trait("Category", "Functional")]
    public async Task RegisterHeartbeat_ShouldBe_Unauthorized()
    {
        //Arrange
        var accessToken = TokenHelper.GetRsaTokenWithIdentityData("testuser1"); // Token without userId claim
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //Act
        var response = await HttpClient.PostAsync("api/v1.0/UserActivity/heartbeat", null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }
}