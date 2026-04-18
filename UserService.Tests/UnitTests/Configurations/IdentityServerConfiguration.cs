using Moq;
using UserService.Application.Exceptions.IdentityServer;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Interfaces.Identity;
using UserService.Tests.Constants;

namespace UserService.Tests.UnitTests.Configurations;

internal static class IdentityServerConfiguration
{
    private static readonly HashSet<string> KnownIdentifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        "testuser1", "testuser2", "testuser3", "testuser5",
        "TestUser1@test.com", "TestUser2@test.com", "TestUser3@test.com",
        "TestUser5@test.com",
        "identityUser", "identityUser@identity.com"
    };

    public static IIdentityServer GetIdentityServerConfiguration()
    {
        var mockIdentityServer = new Mock<IIdentityServer>();

        var randomKeycloakUserTokenDto = new TokenDto
        {
            AccessToken = "newAccessToken",
            AccessExpires = DateTime.UtcNow.AddSeconds(300), //random value
            RefreshToken = "newRefreshToken",
            RefreshExpires = DateTime.UtcNow.AddMinutes(30) //random value
        };

        mockIdentityServer
            .Setup(x => x.FindUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns((string identifier, CancellationToken _) =>
            {
                var result = KnownIdentifiers.Contains(identifier)
                    ? new IdentityUserDto(Guid.NewGuid().ToString(), identifier, identifier)
                    : null;
                return Task.FromResult(result);
            });

        mockIdentityServer
            .Setup(x => x.LoginUserAsync(It.IsAny<IdentityLoginUserDto>(), It.IsAny<CancellationToken>()))
            .Returns((IdentityLoginUserDto dto, CancellationToken _) =>
            {
                if (!KnownIdentifiers.Contains(dto.Identifier))
                    throw new IdentityServerInvalidCredentialsException("TestsIdentityServer", "User not found");

                if (dto.Password == TestConstants.WrongPassword)
                    throw new IdentityServerInvalidCredentialsException("TestsIdentityServer", "Wrong password");

                return Task.FromResult(randomKeycloakUserTokenDto);
            });
        mockIdentityServer
            .Setup(x => x.RegisterUserAsync(It.IsAny<IdentityRegisterUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityRegisterUserDto dto, CancellationToken _) =>
            {
                if (dto.Username == TestConstants.ExistingUsername)
                    throw new IdentityServerConflictException("TestsIdentityServer", "Username already exists");

                return new IdentityUserDto(Guid.NewGuid().ToString(), "testuser", "testEmail@test.com");
            });
        mockIdentityServer.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(randomKeycloakUserTokenDto);
        mockIdentityServer
            .Setup(x => x.UpdateUserAsync(It.IsAny<IdentityUpdateUserDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mockIdentityServer.Object;
    }
}