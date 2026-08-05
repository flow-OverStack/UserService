using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Tests.Mocks;
using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class UserProvisioningServiceTests
{
    [Fact]
    public async Task InitAsync_NewUser_ReturnsSuccess()
    {
        //Arrange
        var provisioningService = new UserProvisioningServiceSut().GetService();
        var dto = new InitUserDto("TestUser4", "TestsUser4@test.com", "test-identity-id-4");

        //Act
        var result = await provisioningService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task InitAsync_InvalidEmail_ReturnsInvalidEmail()
    {
        //Arrange
        var provisioningService = new UserProvisioningServiceSut().GetService();
        var dto = new InitUserDto("TestUser4", "NotEmail", "test-identity-id-4");

        //Act
        var result = await provisioningService.InitAsync(dto);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorMessage.InvalidEmail, result.ErrorMessage);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task InitAsync_ExistingUser_ReturnsSuccess()
    {
        //Arrange
        var provisioningService = new UserProvisioningServiceSut().GetService();
        var dto = new InitUserDto("TestUser1", "TestsUser1@test.com", "test-identity-id-1");

        //Act
        var result = await provisioningService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task InitAsync_NoRolesInRepository_ReturnsRoleNotFound()
    {
        //Arrange
        var provisioningService =
            new UserProvisioningServiceSut(roleRepository: RepositoryMocks.GetEmptyMockRepository<Role>().Object)
                .GetService();
        var dto = new InitUserDto("TestUser4", "TestsUser4@test.com", "test-identity-id-4");

        //Act
        var result = await provisioningService.InitAsync(dto);

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
        var provisioningService = new UserProvisioningServiceSut().GetService();
        var dto = new InitUserDto(username, email, identityId);

        //Act
        var result = await provisioningService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}
