using Moq;
using UserService.Domain.Dtos.Keycloak.User;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Exceptions.IdentityServer;
using UserService.Domain.Interfaces.Service;
using UserService.Tests.Constants;

namespace UserService.Tests.UnitTests.Configurations;

internal static class IdentityServerConfiguration
{
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

        mockIdentityServer.Setup(x => x.LoginUserAsync(It.IsAny<KeycloakLoginUserDto>(), It.IsAny<CancellationToken>()))
            .Returns((KeycloakLoginUserDto dto, CancellationToken _) =>
            {
                if (dto.Password == TestConstants.WrongPassword)
                    throw new IdentityServerPasswordIsWrongException("TestsIdentityServer", "Wrong password");

                return Task.FromResult(randomKeycloakUserTokenDto);
            });
        mockIdentityServer
            .Setup(x => x.RegisterUserAsync(It.IsAny<KeycloakRegisterUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new KeycloakUserDto(Guid.NewGuid()));
        mockIdentityServer.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(randomKeycloakUserTokenDto);

        return mockIdentityServer.Object;
    }
}