using Moq;
using UserService.Application.Resources;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.Role;
using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class RoleServiceTests
{
    [Fact]
    public async Task CreateRoleAsync_NewRoleName_ReturnsSuccess()
    {
        //Arrange
        var roleService = new RoleServiceSut().GetService();
        var dto = new CreateRoleDto("NewTestRole");

        //Act
        var result = await roleService.CreateRoleAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CreateRoleAsync_ExistingRoleName_ReturnsRoleAlreadyExists()
    {
        //Arrange
        var roleService = new RoleServiceSut().GetService();
        var dto = new CreateRoleDto("User");

        //Act
        var result = await roleService.CreateRoleAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleAlreadyExists, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteRoleAsync_ExistingRoleId_ReturnsSuccess()
    {
        //Arrange
        var roleService = new RoleServiceSut().GetService();
        const long roleId = 3;

        //Act
        var result = await roleService.DeleteRoleAsync(roleId);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DeleteRoleAsync_NonExistentRoleId_ReturnsRoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceSut().GetService();
        const long wrongRoleId = 0;

        //Act
        var result = await roleService.DeleteRoleAsync(wrongRoleId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteRoleAsync_DefaultRoleId_ReturnsCannotDeleteDefaultRole()
    {
        //Arrange
        var roleService = new RoleServiceSut().GetService();
        const long roleId = 1;

        //Act
        var result = await roleService.DeleteRoleAsync(roleId);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotDeleteDefaultRole, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleAsync_ExistingRoleId_ReturnsSuccess()
    {
        //Arrange
        var roleService = new RoleServiceSut().GetService();
        var dto = new RoleDto(3, "UpdatedTestRole");

        //Act
        var result = await roleService.UpdateRoleAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateRoleAsync_NonExistentRoleId_ReturnsRoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceSut().GetService();
        var dto = new RoleDto(0, "UpdatedTestRole");

        //Act
        var result = await roleService.UpdateRoleAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}
