using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class GetRoleServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllRoles_ShouldBe_Success()
    {
        //Arrange
        var getRoleService = new GetRoleServiceFactory().GetService();

        //Act
        var result = await getRoleService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetAllRoles_ShouldBe_RolesNotFound()
    {
        //Arrange
        var getRoleService =
            new GetRoleServiceFactory(roleRepository: MockRepositoriesGetters.GetEmptyMockRepository<Role>().Object)
                .GetService();

        //Act
        var result = await getRoleService.GetAllAsync();

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersRoles_ShouldBe_Success()
    {
        //Arrange
        var getRoleService = new GetRoleServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getRoleService.GetUsersRolesAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersRoles_ShouldBe_RolesNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetRoleServiceFactory().GetService();
        var roleIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetUsersRolesAsync(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_Success()
    {
        //Arrange
        var getRoleService = new GetRoleServiceFactory().GetService();
        var roleIds = new List<long> { 1, 2 };

        //Act
        var result = await getRoleService.GetByIdsAsync(roleIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_RoleNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetRoleServiceFactory().GetService();
        var roleIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetByIds_ShouldBe_RolesNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetRoleServiceFactory().GetService();
        var roleIds = new List<long> { 0, 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}