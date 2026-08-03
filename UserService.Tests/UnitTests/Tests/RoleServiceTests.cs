using UserService.Application.Resources;
using UserService.Domain.Dtos.Role;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class RoleServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateRole_ShouldBe_Success()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new CreateRoleDto("NewTestRole");

        //Act
        var result = await roleService.CreateRoleAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task CreateRole_ShouldBe_RoleAlreadyExists()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new CreateRoleDto("User");

        //Act
        var result = await roleService.CreateRoleAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleAlreadyExists, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRole_ShouldBe_Success()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        const long roleId = 3;

        //Act
        var result = await roleService.DeleteRoleAsync(roleId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRole_ShouldBe_RoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        const long wrongRoleId = 0;

        //Act
        var result = await roleService.DeleteRoleAsync(wrongRoleId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRole_ShouldBe_CannotDeleteDefaultRole()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        const long roleId = 1;

        //Act
        var result = await roleService.DeleteRoleAsync(roleId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotDeleteDefaultRole, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRole_ShouldBe_Success()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new RoleDto(3, "UpdatedTestRole");

        //Act
        var result = await roleService.UpdateRoleAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRole_ShouldBe_RoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new RoleDto(0, "UpdatedTestRole");

        //Act
        var result = await roleService.UpdateRoleAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}
