using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ApiTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task PutRole_InvalidClaims_ReturnsForbidden()
    {
        //Arrange
        const string forbiddenUrl = "/api/v1.0/Role";
        var token = TokenHelper.GetRsaToken(2, "testuser2", roles: []);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //Act
        var response = await HttpClient.PutAsync(forbiddenUrl, null);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Invalid claims", body);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task PostRole_MissingAuthToken_ReturnsUnauthorized()
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
    public async Task PostRole_InsufficientPermissions_ReturnsForbidden()
    {
        //Arrange
        const string forbiddenUrl = "/api/v1.0/Role";
        var accessToken = TokenHelper.GetRsaToken();
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
    public async Task GetSwaggerJson_Default_ReturnsOk()
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
    }
}