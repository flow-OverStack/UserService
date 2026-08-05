using UserService.Application.Resources;
using UserService.Domain.Dtos.User;
using UserService.Domain.Entities;
using UserService.Tests.Configurations;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class UserProvisioningServiceTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task InitUser_ShouldBe_Success()
    {
        //Arrange
        var provisioningService = new UserProvisioningServiceFactory().GetService();
        var dto = new InitUserDto("TestUser4", "TestsUser4@test.com", "test-identity-id-4");

        //Act
        var result = await provisioningService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task InitUser_ShouldBe_EmailNotValid()
    {
        //Arrange
        var provisioningService = new UserProvisioningServiceFactory().GetService();
        var dto = new InitUserDto("TestUser4", "NotEmail", "test-identity-id-4");

        //Act
        var result = await provisioningService.InitAsync(dto);

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
        var provisioningService = new UserProvisioningServiceFactory().GetService();
        var dto = new InitUserDto("TestUser1", "TestsUser1@test.com", "test-identity-id-1");

        //Act
        var result = await provisioningService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task InitUser_ShouldBe_RoleNotFound()
    {
        //Arrange
        var provisioningService =
            new UserProvisioningServiceFactory(
                    roleRepository: MockRepositoriesGetters.GetEmptyMockRepository<Role>().Object)
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
    [Trait("Category", "Unit")]
    [InlineData("!@#$%^", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser_LongNameToo", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser1", "TestsUser4@test.com", "test-identity-id-4")]
    [InlineData("TestUser4", "TestsUser4@test.com", "test-identity-id-4")]
    public async Task InitUser_ShouldBe_Success_Username_Variations(string username, string email, string identityId)
    {
        //Arrange
        var provisioningService = new UserProvisioningServiceFactory().GetService();
        var dto = new InitUserDto(username, email, identityId);

        //Act
        var result = await provisioningService.InitAsync(dto);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}
