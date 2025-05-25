using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Resources;
using UserService.Tests.UnitTests.Factories;
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
        var pagination = new PageDto(1, 200);

        //Act
        var result = await getUserService.GetAllAsync(pagination);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
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
    public async Task GetUserByIds_ShouldBe_Success()
    {
        //Arrange
        var getRoleService = new GetUserServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserByIds_ShouldBe_UserNotFound()
    {
        //Arrange
        var getRoleService = new GetUserServiceFactory().GetService();
        var userIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserByIds_ShouldBe_UsersNotFound()
    {
        //Arrange
        var getRoleService = new GetUserServiceFactory().GetService();
        var userIds = new List<long> { 0, 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersWithRoles_ShouldBe_Success()
    {
        //Arrange
        var getRoleService = new GetUserServiceFactory().GetService();
        var roleIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getRoleService.GetUsersWithRolesAsync(roleIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersWithRoles_ShouldBe_UsersNotFound()
    {
        //Arrange
        var getRoleService = new GetUserServiceFactory().GetService();
        var roleIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetUsersWithRolesAsync(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }
}