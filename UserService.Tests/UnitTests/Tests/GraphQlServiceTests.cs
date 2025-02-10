using UserService.Domain.Resources;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class GraphQlServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllUsers_ShouldBe_Success()
    {
        //Arrange
        var graphQlService = new GraphQlServiceFactory().GetService();

        //Act
        var result = await graphQlService.GetAllUsersAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllUsers_ShouldBe_UsersNotFound()
    {
        //Arrange
        var graphQlService = new GraphQlServiceFactory(MockRepositoriesGetters.GetEmptyMockUserRepository().Object)
            .GetService();

        //Act
        var result = await graphQlService.GetAllUsersAsync();


        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllRoles_ShouldBe_Success()
    {
        //Arrange
        var graphQlService = new GraphQlServiceFactory().GetService();

        //Act
        var result = await graphQlService.GetAllRolesAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllRoles_ShouldBe_RolesNotFound()
    {
        //Arrange
        var graphQlService =
            new GraphQlServiceFactory(roleRepository: MockRepositoriesGetters.GetEmptyMockRoleRepository().Object)
                .GetService();

        //Act
        var result = await graphQlService.GetAllRolesAsync();


        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserRoles_ShouldBe_Success()
    {
        //Arrange
        var graphQlService = new GraphQlServiceFactory().GetService();
        const long userId = 1;

        //Act
        var result = await graphQlService.GetUserRoles(userId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserRoles_ShouldBe_UserNotFound()
    {
        //Arrange
        var graphQlService = new GraphQlServiceFactory()
            .GetService();
        const long wrongUserId = 0;

        //Act
        var result = await graphQlService.GetUserRoles(wrongUserId);


        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserRoles_ShouldBe_RolesNotFound()
    {
        //Arrange
        var graphQlService =
            new GraphQlServiceFactory()
                .GetService();
        const long userId = 5;

        //Act
        var result = await graphQlService.GetUserRoles(userId);


        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserWithRole_ShouldBe_Success()
    {
        //Arrange
        var graphQlService =
            new GraphQlServiceFactory(roleRepository: MockRepositoriesGetters.GetMockRoleWithUsersRepository().Object)
                .GetService();
        const long roleId = 1;

        //Act
        var result = await graphQlService.GetUsersWithRole(roleId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserWithRoles_ShouldBe_RoleNotFound()
    {
        //Arrange
        var graphQlService =
            new GraphQlServiceFactory()
                .GetService();
        const long roleId = 4;

        //Act
        var result = await graphQlService.GetUsersWithRole(roleId);


        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserWithRoles_ShouldBe_UsersNotFound()
    {
        //Arrange
        var graphQlService =
            new GraphQlServiceFactory(roleRepository: MockRepositoriesGetters.GetMockRoleWithEmptyUsersRepository()
                    .Object)
                .GetService();
        const long roleId = 1;

        //Act
        var result = await graphQlService.GetUsersWithRole(roleId);


        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }
}