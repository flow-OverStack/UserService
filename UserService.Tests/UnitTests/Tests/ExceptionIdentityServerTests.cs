using UserService.Application.Exceptions.IdentityServer;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Entities;
using UserService.Tests.Constants;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class ExceptionIdentityServerTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_ConflictException()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerFactory().GetService();
        var dto = new IdentityRegisterUserDto(1, "testuser1", "TestsUser1@test.com",
            [new Role { Id = 1, Name = "User" }])
        {
            Password = TestConstants.TestPassword + "1"
        };

        //Act
        var action = async () => await identityServer.RegisterUserAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerConflictException>(action);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task LoginUser_ShouldBe_Exception()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerFactory().GetService();
        var dto = new IdentityLoginUserDto("testuser1", TestConstants.TestPassword + "1");

        //Act
        var action = async () => await identityServer.LoginUserAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerInternalException>(action);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateUser_ShouldBe_Exception()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerFactory().GetService();
        var dto = new IdentityUpdateUserDto(Guid.NewGuid().ToString(), "TestUser1", 1, "NotAEmail",
            [new Role { Id = 1, Name = "User" }, new Role { Id = 2, Name = "Admin" }]);

        //Act
        var action = async () => await identityServer.UpdateUserAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerInternalException>(action);
    }
}