using UserService.Domain.Entity;
using UserService.Domain.Resources;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.ServiceFactories;
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
        Assert.Equal(result.Count, result.Data.Count());
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
        Assert.Equal(0, result.Count);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task GetUsersRoles_ShouldBe_Success()
    {
        //Arrange
        var getRoleService = new GetRoleServiceFactory().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getRoleService.GetUsersRoles(userIds);

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
        var getRoleService = new GetRoleServiceFactory().GetService();
        var roleIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetUsersRoles(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
        Assert.Equal(0, result.Count);
    }
}