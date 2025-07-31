using UserService.Application.Exceptions.IdentityServer;
using UserService.Domain.Dtos.Identity.Role;
using UserService.Domain.Dtos.Identity.User;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Entities;
using UserService.Tests.Constants;
using UserService.Tests.UnitTests.Factories;
using Xunit;

namespace UserService.Tests.UnitTests.Tests;

public class ExceptionIdentityServerTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public async Task RegisterUser_ShouldBe_Exception()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerFactory().GetService();
        var dto = new IdentityRegisterUserDto(4, "testuser4", "TestsUser4@test.com",
            [new Role { Id = 1, Name = "User" }]);

        //Act
        var action = async () => await identityServer.RegisterUserAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerInternalException>(action);
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
    public async Task RefreshToken_ShouldBe_Exception()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerFactory().GetService();
        var dto = new RefreshTokenDto
        {
            RefreshToken = "refresh_token"
        };

        //Act
        var action = async () => await identityServer.RefreshTokenAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerInternalException>(action);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public async Task UpdateRoles_ShouldBe_Exception()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerFactory().GetService();
        var dto = new IdentityUpdateRolesDto(Guid.NewGuid().ToString(), 1, "TestUser1",
            [new Role { Id = 1, Name = "User" }, new Role { Id = 2, Name = "Admin" }]);

        //Act
        var action = async () => await identityServer.UpdateRolesAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerInternalException>(action);
    }
}