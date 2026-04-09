using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Tests.Configurations;
using UserService.Tests.Constants;
using UserService.Tests.UnitTests.Factories;
using Xunit;

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
        var result = await authService.RegisterAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_UsernameNotValid()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new RegisterUserDto("invalid!user", "test@test.com", TestConstants.TestPassword);

        //Act
        var result = await authService.RegisterAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidUsername, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_EmailNotValid()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new RegisterUserDto("TestUser4", "NotEmail", TestConstants.TestPassword + "4");

        //Act
        var result =
            await authService.RegisterAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidEmail, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_PasswordNotValid()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new RegisterUserDto("testuser4", "test@test.com", "123");

        //Act
        var result = await authService.RegisterAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidCredentials, result.ErrorMessage);
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
        var result = await authService.RegisterAsync(dto);

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
            new AuthServiceFactory(roleRepository: MockRepositoriesGetters.GetEmptyMockRepository<Role>().Object)
                .GetService();
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4");

        //Act
        var result = await authService.RegisterAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task InitUser_ShouldBe_Success()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new InitUserDto("TestUser4", "TestsUser4@test.com", "test-identity-id-4");

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task InitUser_ShouldBe_EmailNotValid()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new InitUserDto("TestUser4", "NotEmail", "test-identity-id-4");

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidEmail, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task InitUser_ShouldBe_Success_With_UserAlreadyExists()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new InitUserDto("TestUser1", "TestsUser1@test.com", "test-identity-id-1");

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task InitUser_ShouldBe_RoleNotFound()
    {
        //Arrange
        var authService =
            new AuthServiceFactory(roleRepository: MockRepositoriesGetters.GetEmptyMockRepository<Role>().Object)
                .GetService();
        var dto = new InitUserDto("TestUser4", "TestsUser4@test.com", "test-identity-id-4");

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.RoleNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData("!@#$%^", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser_LongNameToo", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser1", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser4", "TestsUser4@test.com", "test-identity-id-4")]
    public async Task InitUser_ShouldBe_Success_Username_Variations(string username, string email, string identityId)
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new InitUserDto(username, email, identityId);

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUserWithUsername_ShouldBe_Success()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new LoginUserDto("TestUser3",
            TestConstants.TestPassword + "3");

        //Act
        var result =
            await authService.LoginAsync(dto);

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
        var dto = new LoginUserDto("TestUser1@test.com",
            TestConstants.TestPassword + "1");

        //Act
        var result =
            await authService.LoginAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUser_ShouldBe_UserNotFound()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new LoginUserDto("NotExistingUser",
            TestConstants.TestPassword + "1");

        //Act
        var result =
            await authService.LoginAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserNotFound, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUser_ShouldBe_InvalidCredentials()
    {
        //Arrange
        var authService = new AuthServiceFactory().GetService();
        var dto = new LoginUserDto("TestUser1", TestConstants.WrongPassword);

        //Act
        var result =
            await authService.LoginAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidCredentials, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}