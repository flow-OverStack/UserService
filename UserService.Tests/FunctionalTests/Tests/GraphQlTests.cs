using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Newtonsoft.Json;
using UserService.Tests.Extensions;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Configurations.GrpahQl;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class GraphQlTests : BaseFunctionalTest
{
    private const string RequestAllQuery = """
                                           {
                                             users{
                                               id
                                               keycloakId
                                               username
                                               email
                                               lastLoginAt
                                               reputation
                                               createdAt
                                               roles{
                                                 id
                                                 name
                                               }
                                             }
                                             roles{
                                               id
                                               name
                                               users{
                                                 id
                                                 username
                                               }
                                             }
                                           }
                                           """;

    private const string GraphQlEndpoint = "/graphql";

    public GraphQlTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        const string username = "testservice1";
        var serviceToken = TokenExtensions.GetServiceRsaToken(username);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceToken);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_Success()
    {
        //Arrange
        var requestBody = new { query = RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlResponse>(body);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result!.Data.Users);
        Assert.NotNull(result.Data.Roles);
    }
}