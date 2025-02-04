using UserService.Domain.Dto.User;
using UserService.Tests.Extensions;
using UserService.Tests.UnitTests.ServiceFactories;
using Xunit;
using ErrorMessage = UserService.Domain.Resources.ErrorMessage;

namespace UserService.Tests.UnitTests.Tests;

public class AuthServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_Success()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result = await reportService.Register(new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4", TestConstants.TestPassword + "4"));

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_EmailNotValid()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result =
            await reportService.Register(new RegisterUserDto("TestUser4", "NotEmail", TestConstants.TestPassword + "4",
                "TheOtherPassword"));

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.EmailNotValid, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_PasswordMismatch()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result = await reportService.Register(new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4", TestConstants.WrongPassword));

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.PasswordMismatch, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_UserAlreadyExists()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result = await reportService.Register(new RegisterUserDto("TestUser1", "TestsUser1@test.com",
            TestConstants.TestPassword + "1", TestConstants.TestPassword + "1"));

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserAlreadyExists, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUserWithUsername_ShouldBe_Success()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result =
            await reportService.LoginWithUsername(new LoginUsernameUserDto("TestUser3",
                TestConstants.TestPassword + "3"));

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUserWithEmail_ShouldBe_Success()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result =
            await reportService.LoginWithEmail(new LoginEmailUserDto("TestUser1@test.com",
                TestConstants.TestPassword + "1"));

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUserWithEmail_ShouldBe_EmailNotValid()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result =
            await reportService.LoginWithEmail(new LoginEmailUserDto("NotEmail", TestConstants.TestPassword + "1"));
        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.EmailNotValid, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUser_ShouldBe_UserNotFound()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result =
            await reportService.LoginWithUsername(new LoginUsernameUserDto("NotExistingUser",
                TestConstants.TestPassword + "1"));

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUser_ShouldBe_PasswordIsWrong()
    {
        //Arrange
        var reportService = new AuthServiceFactory().GetService();
        //Act
        var result =
            await reportService.LoginWithUsername(new LoginUsernameUserDto("TestUser1", TestConstants.WrongPassword));

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.PasswordIsWrong, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}