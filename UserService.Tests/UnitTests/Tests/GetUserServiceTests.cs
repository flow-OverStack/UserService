using UserService.Domain.Entity;
using UserService.Domain.Resources;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class GetUserServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllUsers_ShouldBe_Success()
    {
        //Arrange
        var getUserService = new GetUserServiceFactory().GetService();

        //Act
        var result = await getUserService.GetAllAsync();

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
        var getUserService = new GetUserServiceFactory(MockRepositoriesGetters.GetEmptyMockRepository<User>().Object)
            .GetService();

        //Act
        var result = await getUserService.GetAllAsync();

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserById_ShouldBe_Success()
    {
        //Arrange
        const long userId = 1;
        var getUserService = new GetUserServiceFactory().GetService();

        //Act
        var result = await getUserService.GetByIdAsync(userId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserById_ShouldBe_UserNotFound()
    {
        //Arrange
        const long userId = 0;
        var getUserService = new GetUserServiceFactory().GetService();

        //Act
        var result = await getUserService.GetByIdAsync(userId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserWithRole_ShouldBe_Success()
    {
        //Arrange
        var getUserService =
            new GetUserServiceFactory()
                .GetService();
        const long roleId = 1;

        //Act
        var result = await getUserService.GetUsersWithRole(roleId);

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
        var getUserService = new GetUserServiceFactory().GetService();
        const long roleId = 4;

        //Act
        var result = await getUserService.GetUsersWithRole(roleId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }
}