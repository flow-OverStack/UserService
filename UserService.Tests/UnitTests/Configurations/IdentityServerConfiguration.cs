using Moq;
using UserService.Application.Exceptions.IdentityServer;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Interfaces.Identity;
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

        mockIdentityServer.Setup(x => x.LoginUserAsync(It.IsAny<IdentityLoginUserDto>(), It.IsAny<CancellationToken>()))
            .Returns((IdentityLoginUserDto dto, CancellationToken _) =>
            {
                if (dto.Password == TestConstants.WrongPassword)
                    throw new IdentityServerInvalidCredentialsException("TestsIdentityServer", "Wrong password");

                return Task.FromResult(randomKeycloakUserTokenDto);
            });
        mockIdentityServer
            .Setup(x => x.RegisterUserAsync(It.IsAny<IdentityRegisterUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentityUserDto(Guid.NewGuid().ToString(), "testuser", "testEmail@test.com"));
        mockIdentityServer.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(randomKeycloakUserTokenDto);
        mockIdentityServer
            .Setup(x => x.UpdateUserAsync(It.IsAny<IdentityUpdateUserDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mockIdentityServer.Object;
    }
}