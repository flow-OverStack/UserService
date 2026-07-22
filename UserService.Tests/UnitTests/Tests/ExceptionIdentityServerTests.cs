using UserService.Application.Exceptions.IdentityServer;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Entities;
using UserService.Tests.Constants;
using UserService.Tests.UnitTests.Sut;
using Xunit;
using UserService.Tests.Traits;

namespace UserService.Tests.UnitTests.Tests;

[UnitTest]
public class ExceptionIdentityServerTests
{
    [Fact]
    public async Task RegisterUserAsync_ConflictingUser_ThrowsIdentityServerConflictException()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerSut().GetService();
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

    [Fact]
    public async Task LoginUserAsync_ServerError_ThrowsIdentityServerInternalException()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerSut().GetService();
        var dto = new IdentityLoginUserDto("testuser1", TestConstants.TestPassword + "1");

        //Act
        var action = async () => await identityServer.LoginUserAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerInternalException>(action);
    }

    [Fact]
    public async Task UpdateUserAsync_ServerError_ThrowsIdentityServerInternalException()
    {
        //Arrange
        var identityServer = new ExceptionIdentityServerSut().GetService();
        var dto = new IdentityUpdateUserDto(Guid.NewGuid().ToString(), "TestUser1", 1, "NotAEmail",
            [new Role { Id = 1, Name = "User" }, new Role { Id = 2, Name = "Admin" }]);

        //Act
        var action = async () => await identityServer.UpdateUserAsync(dto);

        //Assert
        await Assert.ThrowsAsync<IdentityServerInternalException>(action);
    }
}