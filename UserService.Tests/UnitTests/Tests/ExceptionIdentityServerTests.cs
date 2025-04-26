using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Dto.Token;
using UserService.Domain.Entity;
using UserService.Domain.Exceptions.IdentityServer;
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
        var dto = new KeycloakRegisterUserDto(4, "testuser4", "TestsUser4@test.com",
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
        var dto = new KeycloakLoginUserDto("testuser1")
        {
            Password = TestConstants.TestPassword + "1"
        };

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
        var dto = new KeycloakUpdateRolesDto
        {
            UserId = 1,
            KeycloakUserId = Guid.NewGuid(),
            Email = "TestUser1@test.com",
            NewRoles = [new Role { Id = 1, Name = "User" }, new Role { Id = 2, Name = "Admin" }]
        };

        //Act
        var action = async () => await identityServer.UpdateRolesAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerInternalException>(action);
    }
}