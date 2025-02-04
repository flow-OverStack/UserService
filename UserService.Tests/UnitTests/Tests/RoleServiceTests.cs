using UserService.Domain.Dto.Role;
using UserService.Domain.Dto.UserRole;
using UserService.Domain.Resources;
using UserService.Tests.UnitTests.ServiceFactories;
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
        //Act
        var result = await roleService.CreateRoleAsync(new CreateRoleDto("NewTestRole"));

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
        //Act
        var result = await roleService.CreateRoleAsync(new CreateRoleDto("User"));

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleAlreadyExists);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRole_ShouldBe_Success()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        //Act
        var result = await roleService.DeleteRoleAsync(3);

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
        //Act
        var result = await roleService.DeleteRoleAsync(0);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRole_ShouldBe_Success()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        //Act
        var result = await roleService.UpdateRoleAsync(new RoleDto(3, "UpdatedTestRole"));

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
        //Act
        var result = await roleService.UpdateRoleAsync(new RoleDto(0, "UpdatedTestRole"));

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_Success()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();

        //Act
        var result = await roleService.AddRoleForUserAsync(new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 3
        });


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
        //Act
        var result = await roleService.AddRoleForUserAsync(new UserRoleDto
        {
            Username = "NotExistingUser",
            RoleId = 3
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.UserNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_UserAlreadyHasThisRole()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        //Act
        var result = await roleService.AddRoleForUserAsync(new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 1
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.UserAlreadyHasThisRole);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task AddRoleForUser_ShouldBe_RoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        //Act
        var result = await roleService.AddRoleForUserAsync(new UserRoleDto
        {
            Username = "TestUser1",
            RoleId = 0
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_Success()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();

        //Act
        var result = await roleService.DeleteRoleForUserAsync(new DeleteUserRoleDto
        {
            Username = "TestUser2",
            RoleId = 3
        });


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
        //Act
        var result = await roleService.DeleteRoleForUserAsync(new DeleteUserRoleDto
        {
            Username = "NotExistingUser",
            RoleId = 3
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.UserNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task DeleteRoleForUser_ShouldBe_RoleNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        //Act
        var result = await roleService.DeleteRoleForUserAsync(new DeleteUserRoleDto
        {
            Username = "TestUser2",
            RoleId = 0
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleNotFound);
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
        //Act
        var result = await roleService.UpdateRoleForUserAsync(new UpdateUserRoleDto
        {
            Username = "NotExistingUser",
            FromRoleId = 3,
            ToRoleId = 2
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.UserNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_RoleToBeUpdatedIsNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        //Act
        var result = await roleService.UpdateRoleForUserAsync(new UpdateUserRoleDto
        {
            Username = "TestUser2",
            FromRoleId = 0,
            ToRoleId = 2
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleToBeUpdatedIsNotFound);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_UserAlreadyHasThisRole()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        //Act
        var result = await roleService.UpdateRoleForUserAsync(new UpdateUserRoleDto
        {
            Username = "TestUser1",
            FromRoleId = 2,
            ToRoleId = 1
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.UserAlreadyHasThisRole);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoleForUser_ShouldBe_RoleToUpdateIsNotFound()
    {
        //Arrange
        var roleService = new RoleServiceFactory().GetService();
        //Act
        var result = await roleService.UpdateRoleForUserAsync(new UpdateUserRoleDto
        {
            Username = "TestUser2",
            FromRoleId = 3,
            ToRoleId = 0
        });

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(result.ErrorMessage, ErrorMessage.RoleToUpdateIsNotFound);
        Assert.Null(result.Data);
    }
}