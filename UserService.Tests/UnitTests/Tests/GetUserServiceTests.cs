using UserService.Application.Resources;
using UserService.Domain.Settings;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class GetUserServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllUsers_NoFilter_ReturnsSuccess()
    {
        //Arrange
        var getUserService = new CacheGetUserServiceFactory().GetService();

        //Act
        var result = await getUserService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserByIds_MixOfExistingAndNonExistentIds_ReturnsSuccess()
    {
        //Arrange
        var getRoleService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserByIds_SingleNonExistentId_ReturnsUserNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUserByIds_MultipleNonExistentIds_ReturnsUsersNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 0, 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersWithRoles_MixOfExistingAndNonExistentRoleIds_ReturnsSuccess()
    {
        //Arrange
        var getRoleService = new CacheGetUserServiceFactory().GetService();
        var roleIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getRoleService.GetUsersWithRolesAsync(roleIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersWithRoles_NonExistentRoleIds_ReturnsUsersNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetUserServiceFactory().GetService();
        var roleIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetUsersWithRolesAsync(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetCurrentReputationsAsync_MixOfExistingAndNonExistentIds_ReturnsSuccess()
    {
        //Arrange
        var getUserService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getUserService.GetCurrentReputationsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.All(result.Data, kvp => Assert.True(kvp.Value >= BusinessRules.MinReputation));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetCurrentReputationsAsync_SingleNonExistentId_ReturnsUserNotFound()
    {
        //Arrange
        var getUserService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 0 };

        //Act
        var result = await getUserService.GetCurrentReputationsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetCurrentReputationsAsync_MultipleNonExistentIds_ReturnsUsersNotFound()
    {
        //Arrange
        var getUserService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 0, 0 };

        //Act
        var result = await getUserService.GetCurrentReputationsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetRemainingReputationsAsync_MixOfExistingAndNonExistentIds_ReturnsSuccess()
    {
        //Arrange
        var getUserService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getUserService.GetRemainingReputationsAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.All(result.Data, kvp => Assert.InRange(kvp.Value, 0, BusinessRules.MaxDailyReputation));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetRemainingReputationsAsync_SingleNonExistentId_ReturnsUserNotFound()
    {
        //Arrange
        var getUserService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 0 };

        //Act
        var result = await getUserService.GetRemainingReputationsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetRemainingReputationsAsync_MultipleNonExistentIds_ReturnsUsersNotFound()
    {
        //Arrange
        var getUserService = new CacheGetUserServiceFactory().GetService();
        var userIds = new List<long> { 0, 0 };

        //Act
        var result = await getUserService.GetRemainingReputationsAsync(userIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsersNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}