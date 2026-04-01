using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Tests.FunctionalTests.Base.Exception.GraphQl;
using UserService.Tests.FunctionalTests.Configurations.GraphQl.Responses;
using UserService.Tests.FunctionalTests.Helpers;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class GraphQlMeExceptionTests(NullHttpContextMeGraphQlTestWebAppFactory factory)
    : IClassFixture<NullHttpContextMeGraphQlTestWebAppFactory>
{
    private readonly HttpClient _httpClient = factory.CreateClient();

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetMe_ShouldBe_NullContextError()
    {
        //Arrange
        var token = TokenHelper.GetRsaToken(1, "testuser1");
        var request = new HttpRequestMessage(HttpMethod.Post, GraphQlHelper.GraphQlEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { query = GraphQlHelper.RequestMeQuery });

        //Act
        var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body)!;

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, x => x.Message == "User is not authenticated.");
    }
}