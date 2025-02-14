using System.Net;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class ApiTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task RegisterUser_ShouldBe_Success()
    {
        //Arrange
        const string wrongUrl = "wrongUrl";

        //Act
        var response = await HttpClient.GetAsync(wrongUrl);
        var body = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(body);
        Assert.Contains($"{(int)HttpStatusCode.NotFound} {HttpStatusCode.NotFound}", body);
    }
}