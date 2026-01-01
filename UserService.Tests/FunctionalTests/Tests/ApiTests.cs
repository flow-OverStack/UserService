using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using UserService.Domain.Entities;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ApiTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestForbiddenResource_ShouldBe_Unauthorized()
    {
        //Arrange
        const string forbiddenUrl = "/api/v1.0/Role";

        //Act
        var response = await HttpClient.PostAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestForbiddenResource_ShouldBe_Forbidden()
    {
        //Arrange
        const string forbiddenUrl = "/api/v1.0/Role";
        var accessToken = TokenHelper.GetRsaTokenWithRoleClaims("testuser1", [
            new Role { Name = "User" }
        ]);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //Act
        var response = await HttpClient.PostAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(MediaTypeNames.Text.Plain, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task RequestSwagger_ShouldBe_Success()
    {
        //Arrange
        const string swaggerUrl = "/swagger/v1/swagger.json";

        //Act
        var response = await HttpClient.GetAsync(swaggerUrl);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(MediaTypeNames.Application.Json, response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Contains("\"deprecated\": true", body);
    }
}