using UserService.Application.Resources;
using UserService.Domain.Dtos.UserRole;
using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class UserRoleServiceTests
{
    [Fact]
    public async Task AddRoleForUserAsync_ExistingUserAndRole_ReturnsSuccess()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 3
        };

        //Act
        var result = await userRoleService.AddRoleForUserAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task AddRoleForUserAsync_NonExistentUsername_ReturnsUserNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UserRoleDto
        {
            Username = "NotExistingUser",
            RoleId = 3
        };

        //Act
        var result = await userRoleService.AddRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task AddRoleForUserAsync_UserAlreadyInRole_ReturnsUserAlreadyHasThisRole()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 1
        };

        //Act
        var result = await userRoleService.AddRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserAlreadyHasThisRole, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task AddRoleForUserAsync_NonExistentRoleId_ReturnsRoleNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 0
        };

        //Act
        var result = await userRoleService.AddRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteRoleForUserAsync_ExistingUserAndRole_ReturnsSuccess()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser2",
            RoleId = 3
        };

        //Act
        var result = await userRoleService.DeleteRoleForUserAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task DeleteRoleForUserAsync_NonExistentUsername_ReturnsUserNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UserRoleDto
        {
            Username = "NotExistingUser",
            RoleId = 3
        };

        //Act
        var result = await userRoleService.DeleteRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteRoleForUserAsync_NonExistentRoleId_ReturnsRoleNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser2",
            RoleId = 0
        };

        //Act
        var result = await userRoleService.DeleteRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task DeleteRoleForUserAsync_DefaultRoleId_ReturnsCannotDeleteDefaultRole()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UserRoleDto
        {
            Username = "TestUser2",
            RoleId = 1
        };

        //Act
        var result = await userRoleService.DeleteRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.CannotDeleteDefaultRole, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_ExistingUserAndRoles_ReturnsSuccess()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();

        //Act
        var result = await userRoleService.UpdateRoleForUserAsync(new UpdateUserRoleDto
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
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UpdateUserRoleDto
        {
            Username = "NotExistingUser",
            FromRoleId = 3,
            ToRoleId = 2
        };

        //Act
        var result = await userRoleService.UpdateRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_NonExistentFromRoleId_ReturnsRoleToBeUpdatedIsNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UpdateUserRoleDto
        {
            Username = "TestUser2",
            FromRoleId = 0,
            ToRoleId = 2
        };

        //Act
        var result = await userRoleService.UpdateRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleToBeUpdatedIsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_UserAlreadyInToRole_ReturnsUserAlreadyHasThisRole()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UpdateUserRoleDto
        {
            Username = "TestUser1",
            FromRoleId = 2,
            ToRoleId = 1
        };

        //Act
        var result = await userRoleService.UpdateRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserAlreadyHasThisRole, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateRoleForUserAsync_NonExistentToRoleId_ReturnsRoleToUpdateIsNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceSut().GetService();
        var dto = new UpdateUserRoleDto
        {
            Username = "TestUser2",
            FromRoleId = 3,
            ToRoleId = 0
        };

        //Act
        var result = await userRoleService.UpdateRoleForUserAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleToUpdateIsNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}
