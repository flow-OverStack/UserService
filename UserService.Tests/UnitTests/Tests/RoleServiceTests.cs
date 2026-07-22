using UserService.Application.Resources;
using UserService.Domain.Dtos.Role;
using UserService.Domain.Dtos.UserRole;
using UserService.Tests.UnitTests.Factories;
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
        var roleService = new RoleServiceFactory().GetService();
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
        var roleService = new RoleServiceFactory().GetService();
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
        var roleService = new RoleServiceFactory().GetService();
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
        var roleService = new RoleServiceFactory().GetService();
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
        var roleService = new RoleServiceFactory().GetService();
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
        var roleService = new RoleServiceFactory().GetService();
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
        var roleService = new RoleServiceFactory().GetService();
        var dto = new RoleDto(0, "UpdatedTestRole");

        //Act
        var result = await roleService.UpdateRoleAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task AddRoleForUserAsync_ExistingUserAndRole_ReturnsSuccess()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 3
        };

        //Act
        var result = await roleService.AddRoleForUserAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task AddRoleForUserAsync_NonExistentUsername_ReturnsUserNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UserRoleDto
        {
            Username = "NotExistingUser",
            RoleId = 3
        };

        //Act
        var result = await roleService.AddRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task AddRoleForUserAsync_UserAlreadyInRole_ReturnsUserAlreadyHasThisRole()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 1
        };

        //Act
        var result = await roleService.AddRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserAlreadyHasThisRole, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task AddRoleForUserAsync_NonExistentRoleId_ReturnsRoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 0
        };

        //Act
        var result = await roleService.AddRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteRoleForUserAsync_ExistingUserAndRole_ReturnsSuccess()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser2",
            RoleId = 3
        };

        //Act
        var result = await roleService.DeleteRoleForUserAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DeleteRoleForUserAsync_NonExistentUsername_ReturnsUserNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UserRoleDto
        {
            Username = "NotExistingUser",
            RoleId = 3
        };

        //Act
        var result = await roleService.DeleteRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteRoleForUserAsync_NonExistentRoleId_ReturnsRoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser2",
            RoleId = 0
        };

        //Act
        var result = await roleService.DeleteRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteRoleForUserAsync_DefaultRoleId_ReturnsCannotDeleteDefaultRole()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser2",
            RoleId = 1
        };

        //Act
        var result = await roleService.DeleteRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotDeleteDefaultRole, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_ExistingUserAndRoles_ReturnsSuccess()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();

        //Act
        var result = await roleService.UpdateRoleForUserAsync(new UpdateUserRoleDto
        {
            Username = "TestUser2",
            FromRoleId = 3,
            ToRoleId = 2
        });

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_NonExistentUsername_ReturnsUserNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UpdateUserRoleDto
        {
            Username = "NotExistingUser",
            FromRoleId = 3,
            ToRoleId = 2
        };

        //Act
        var result = await roleService.UpdateRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_NonExistentFromRoleId_ReturnsRoleToBeUpdatedIsNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UpdateUserRoleDto
        {
            Username = "TestUser2",
            FromRoleId = 0,
            ToRoleId = 2
        };

        //Act
        var result = await roleService.UpdateRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleToBeUpdatedIsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_UserAlreadyInToRole_ReturnsUserAlreadyHasThisRole()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UpdateUserRoleDto
        {
            Username = "TestUser1",
            FromRoleId = 2,
            ToRoleId = 1
        };

        //Act
        var result = await roleService.UpdateRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserAlreadyHasThisRole, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_NonExistentToRoleId_ReturnsRoleToUpdateIsNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new UpdateUserRoleDto
        {
            Username = "TestUser2",
            FromRoleId = 3,
            ToRoleId = 0
        };

        //Act
        var result = await roleService.UpdateRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleToUpdateIsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}