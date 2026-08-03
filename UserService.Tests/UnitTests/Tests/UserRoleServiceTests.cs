using UserService.Application.Resources;
using UserService.Domain.Dtos.UserRole;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class UserRoleServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_Success()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_UserNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_UserAlreadyHasThisRole()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_RoleNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_Success()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_UserNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_RoleNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_CannotDeleteDefaultRole()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_Success()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();

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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_UserNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_RoleToBeUpdatedIsNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_UserAlreadyHasThisRole()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_RoleToUpdateIsNotFound()
    {
        //Arrange
        var userRoleService = new UserRoleServiceFactory().GetService();
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
