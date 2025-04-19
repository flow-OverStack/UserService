using GreenDonut;
using HotChocolate;
using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Resources;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class RoleDataLoaderTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<RoleDataLoader>();
        var roleIds = new List<long>
            { 1, 2 }; //When LoadRequiredAsync if some keys were not resolved the exception is thrown 

        //Act
        var roles = await dataLoader.LoadRequiredAsync(roleIds);

        //Assert
        Assert.Equal(roles.Count, roleIds.Count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_RoleNotFound()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<RoleDataLoader>();
        var roleIds = new List<long> { 0 };

        //Act
        var action = async () => await dataLoader.LoadRequiredAsync(roleIds);

        //Assert
        var exception = await Assert.ThrowsAsync<GraphQLException>(action);
        Assert.Equal(ErrorMessage.RoleNotFound, exception.Message);
    }
}