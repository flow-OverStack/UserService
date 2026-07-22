using UserService.Application.Resources;
using UserService.Domain.Entities;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class GetRoleServiceTests
{
    [Fact]
    public async Task GetAllRoles_NoFilter_ReturnsSuccess()
    {
        //Arrange
        var getRoleService = new GetRoleServiceSut().GetService();

        //Act
        var result = await getRoleService.GetAllAsync();

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetAllRoles_EmptyRepository_ReturnsRolesNotFound()
    {
        //Arrange
        var getRoleService =
            new GetRoleServiceSut(roleRepository: RepositoryMocks.GetEmptyMockRepository<Role>().Object)
                .GetService();

        //Act
        var result = await getRoleService.GetAllAsync();

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetUsersRoles_MixOfExistingAndNonExistentUserIds_ReturnsSuccess()
    {
        //Arrange
        var getRoleService = new GetRoleServiceSut().GetService();
        var userIds = new List<long> { 1, 2, 0 };

        //Act
        var result = await getRoleService.GetUsersRolesAsync(userIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(result.Count, result.Data.Count());
    }

    [Fact]
    public async Task GetUsersRoles_NonExistentUserIds_ReturnsRolesNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetRoleServiceSut().GetService();
        var roleIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetUsersRolesAsync(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByIds_ExistingIds_ReturnsSuccess()
    {
        //Arrange
        var getRoleService = new GetRoleServiceSut().GetService();
        var roleIds = new List<long> { 1, 2 };

        //Act
        var result = await getRoleService.GetByIdsAsync(roleIds);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetByIds_SingleNonExistentId_ReturnsRoleNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetRoleServiceSut().GetService();
        var roleIds = new List<long> { 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByIds_MultipleNonExistentIds_ReturnsRolesNotFound()
    {
        //Arrange
        var getRoleService = new CacheGetRoleServiceSut().GetService();
        var roleIds = new List<long> { 0, 0 };

        //Act
        var result = await getRoleService.GetByIdsAsync(roleIds);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RolesNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}