using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class UserServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateUsername_ShouldBe_Success()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateUsername_ShouldBe_Success_With_UsernameNotChanged()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateUsername_ShouldBe_InvalidUsername()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateUsername_ShouldBe_UserNotFound()
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

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateUsername_ShouldBe_UsernameAlreadyTaken()
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