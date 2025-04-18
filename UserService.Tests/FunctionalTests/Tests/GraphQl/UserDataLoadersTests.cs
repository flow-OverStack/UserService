using GreenDonut;
using HotChocolate;
using Microsoft.Extensions.DependencyInjection;
using UserService.Domain.Resources;
using UserService.GraphQl.DataLoaders;
using UserService.Tests.FunctionalTests.Base;
using Xunit;

namespace UserService.Tests.FunctionalTests.Tests.GraphQl;

public class UserDataLoadersTests(FunctionalTestWebAppFactory factory) : BaseFunctionalTest(factory)
{
    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_Success()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<UserDataLoader>();
        var userIds = new List<long>
            { 1, 2 }; //When LoadRequiredAsync if some keys were not resolved the exception is thrown 

        //Act
        var users = await dataLoader.LoadRequiredAsync(userIds);

        //Assert
        Assert.Equal(users.Count, userIds.Count);
    }

    [Trait("Category", "Functional")]
    [Fact]
    public async Task LoadBatch_ShouldBe_UsersNotFound()
    {
        //Arrange
        using var scope = ServiceProvider.CreateScope();
        var dataLoader = scope.ServiceProvider.GetRequiredService<UserDataLoader>();
        var userIds = new List<long> { 0 };

        //Act
        var action = async () => await dataLoader.LoadRequiredAsync(userIds);

        //Assert
        var exception = await Assert.ThrowsAsync<GraphQLException>(action);
        Assert.Equal(ErrorMessage.UsersNotFound, exception.Message);
    }
}