using UserService.Domain.Dto.User;
using UserService.Tests.Configurations;
using UserService.Tests.Constants;
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
        var authService = new AuthServiceFactory().GetService();
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4");

        //Act
        var result = await authService.Register(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_EmailNotValid()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new RegisterUserDto("TestUser4", "NotEmail",
            TestConstants.TestPassword + "4");

        //Act
        var result =
            await authService.Register(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.EmailNotValid, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_UserAlreadyExists()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new RegisterUserDto("TestUser1", "TestsUser1@test.com",
            TestConstants.TestPassword + "1");

        //Act
        var result = await authService.Register(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserAlreadyExists, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_RoleNotFound()
    {
        //Arrange
        var authService =
            new AuthServiceFactory(roleRepository: MockRepositoriesGetters.GetEmptyMockRoleRepository().Object)
                .GetService();
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4");

        //Act
        var result = await authService.Register(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUserWithUsername_ShouldBe_Success()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new LoginUsernameUserDto("TestUser3",
            TestConstants.TestPassword + "3");

        //Act
        var result =
            await authService.LoginWithUsername(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUserWithEmail_ShouldBe_Success()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new LoginEmailUserDto("TestUser1@test.com",
            TestConstants.TestPassword + "1");

        //Act
        var result =
            await authService.LoginWithEmail(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUserWithEmail_ShouldBe_EmailNotValid()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new LoginEmailUserDto("NotEmail", TestConstants.TestPassword + "1");

        //Act
        var result =
            await authService.LoginWithEmail(dto);

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
        var authService = new AuthServiceFactory().GetService();
        var dto = new LoginUsernameUserDto("NotExistingUser",
            TestConstants.TestPassword + "1");

        //Act
        var result =
            await authService.LoginWithUsername(dto);

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
        var authService = new AuthServiceFactory().GetService();
        var dto = new LoginUsernameUserDto("TestUser1", TestConstants.WrongPassword);

        //Act
        var result =
            await authService.LoginWithUsername(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.PasswordIsWrong, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}