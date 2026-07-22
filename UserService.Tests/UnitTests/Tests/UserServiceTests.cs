using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Tests.UnitTests.Factories;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class UserServiceTests
{
    [Fact]
    public async Task UpdateUsernameAsync_NewUsername_ReturnsSuccess()
    {
        //Arrange
        var userService = new UserServiceFactory().GetService();
        var dto = new UpdateUsernameDto(1, "newusername");

        //Act
        var result = await userService.UpdateUsernameAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("newusername", result.Data.Username);
    }

    [Fact]
    public async Task UpdateUsernameAsync_UsernameUnchanged_ReturnsSuccess()
    {
        //Arrange
        var userService = new UserServiceFactory().GetService();
        var dto = new UpdateUsernameDto(1, "testuser1");

        //Act
        var result = await userService.UpdateUsernameAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("testuser1", result.Data.Username);
    }

    [Fact]
    public async Task UpdateUsernameAsync_InvalidUsernameFormat_ReturnsInvalidUsername()
    {
        //Arrange
        var userService = new UserServiceFactory().GetService();
        var dto = new UpdateUsernameDto(1, "invalid!user");

        //Act
        var result = await userService.UpdateUsernameAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidUsername, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateUsernameAsync_NonExistentUserId_ReturnsUserNotFound()
    {
        //Arrange
        var userService = new UserServiceFactory().GetService();
        var dto = new UpdateUsernameDto(0, "newusername");

        //Act
        var result = await userService.UpdateUsernameAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateUsernameAsync_DuplicateUsername_ReturnsUsernameAlreadyTaken()
    {
        //Arrange
        var userService = new UserServiceFactory().GetService();
        var dto = new UpdateUsernameDto(1, "testuser2");

        //Act
        var result = await userService.UpdateUsernameAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UsernameAlreadyTaken, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}