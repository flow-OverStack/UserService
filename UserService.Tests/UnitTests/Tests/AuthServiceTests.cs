using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Tests.Constants;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ValidNewUser_ReturnsSuccess()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new RegisterUserDto("TestUser4", "TestsUser4@test.com",
            TestConstants.TestPassword + "4");

        //Act
        var result = await authService.RegisterAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task RegisterAsync_InvalidUsername_ReturnsInvalidUsername()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new RegisterUserDto("invalid!user", "test@test.com", TestConstants.TestPassword);

        //Act
        var result = await authService.RegisterAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidUsername, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RegisterAsync_InvalidEmail_ReturnsInvalidEmail()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new RegisterUserDto("TestUser4", "NotEmail", TestConstants.TestPassword + "4");

        //Act
        var result =
            await authService.RegisterAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidEmail, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RegisterAsync_InvalidPassword_ReturnsInvalidCredentials()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new RegisterUserDto("testuser4", "test@test.com", "123");

        //Act
        var result = await authService.RegisterAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidCredentials, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Theory]
    [InlineData("TestUser1", "TestsUser1@test.com")]
    [InlineData("identityUser", "usernameTest@test.com")]
    [InlineData("emailTest", "identityUser@identity.com")]
    [InlineData(TestConstants.ExistingUsername, "usernameTest@test.com")]
    public async Task RegisterAsync_ExistingUsernameOrEmail_ReturnsUserAlreadyExists(string username, string email)
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new RegisterUserDto(username, email, TestConstants.TestPassword + "1");

        //Act
        var result = await authService.RegisterAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.UserAlreadyExists, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task RegisterAsync_NoRolesInRepository_ReturnsRoleNotFound()
    {
        //Arrange
        var authService =
            new AuthServiceSut(roleRepository: RepositoryMocks.GetEmptyMockRepository<Role>().Object)
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

    [Fact]
    public async Task InitAsync_NewUser_ReturnsSuccess()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new InitUserDto("TestUser4", "TestsUser4@test.com", "test-identity-id-4");

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task InitAsync_InvalidEmail_ReturnsInvalidEmail()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new InitUserDto("TestUser4", "NotEmail", "test-identity-id-4");

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidEmail, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task InitAsync_ExistingUser_ReturnsSuccess()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new InitUserDto("TestUser1", "TestsUser1@test.com", "test-identity-id-1");

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task InitAsync_NoRolesInRepository_ReturnsRoleNotFound()
    {
        //Arrange
        var authService =
            new AuthServiceSut(roleRepository: RepositoryMocks.GetEmptyMockRepository<Role>().Object)
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
    [InlineData("!@#$%^", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser_LongNameToo", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser1", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser4", "TestsUser4@test.com", "test-identity-id-4")]
    public async Task InitAsync_UsernameVariations_ReturnsSuccess(string username, string email, string identityId)
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new InitUserDto(username, email, identityId);

        //Act
        var result = await authService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task LoginAsync_ValidUsernameCredentials_ReturnsSuccess()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new LoginUserDto("TestUser1", TestConstants.TestPassword + "1");

        //Act
        var result = await authService.LoginAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task LoginAsync_ValidEmailCredentials_ReturnsSuccess()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new LoginUserDto("TestUser1@test.com", TestConstants.TestPassword + "1");

        //Act
        var result = await authService.LoginAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task LoginAsync_NonExistentUser_ReturnsInvalidCredentials()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new LoginUserDto("NotExistingUser", TestConstants.TestPassword + "1");

        //Act
        var result = await authService.LoginAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidCredentials, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsInvalidCredentials()
    {
        //Arrange
        var authService = new AuthServiceSut().GetService();
        var dto = new LoginUserDto("TestUser1", TestConstants.WrongPassword);

        //Act
        var result = await authService.LoginAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidCredentials, result.ErrorMessage);
        Assert.Null(result.Data);
    }
}