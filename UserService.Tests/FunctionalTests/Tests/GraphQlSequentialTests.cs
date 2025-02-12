using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using UserService.DAL;
using UserService.Domain.Entity;
using UserService.Domain.Resources;
using UserService.Tests.Constants;
using UserService.Tests.Extensions;
using UserService.Tests.FunctionalTests.Base;
using UserService.Tests.FunctionalTests.Configurations.GrpahQl;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests;

public class GraphQlSequentialTests : SequentialFunctionalTest
{
    public GraphQlSequentialTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
        const string username = "testservice1";
        var serviceToken = TokenExtensions.GetServiceRsaToken(username);
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceToken);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task GetAll_ShouldBe_NotFoundError()
    {
        //Arrange
        DeleteUsersAndRoles();
        var requestBody = new { query = GraphQlConstants.RequestAllQuery };

        //Act
        var response = await HttpClient.PostAsJsonAsync(GraphQlConstants.GraphQlEndpoint, requestBody);
        var body = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GraphQlErrorResponse>(body);

        var areUsersNotFound = result!.Errors.Exists(x => x.Message == ErrorMessage.UsersNotFound);
        var areRolesNotFound = result.Errors.Exists(x => x.Message == ErrorMessage.RolesNotFound);

        //Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(areUsersNotFound);
        Assert.True(areRolesNotFound);
    }

    private void DeleteUsersAndRoles()
    {
        using var scope = ServicesProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Set<User>().ExecuteDelete();
        dbContext.Set<Role>().ExecuteDelete();

        dbContext.SaveChanges();
    }
}