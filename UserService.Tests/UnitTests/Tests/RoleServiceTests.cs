using UserService.Domain.Dtos.Role;
using UserService.Domain.Dtos.UserRole;
using UserService.Domain.Resources;
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_Success()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_UserNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_UserAlreadyHasThisRole()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_RoleNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_Success()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new DeleteUserRoleDto
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_UserNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new DeleteUserRoleDto
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_RoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new DeleteUserRoleDto
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_CannotDeleteDefaultRole()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        var dto = new DeleteUserRoleDto
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_Success()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_UserNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_RoleToBeUpdatedIsNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_UserAlreadyHasThisRole()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_RoleToUpdateIsNotFound()
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