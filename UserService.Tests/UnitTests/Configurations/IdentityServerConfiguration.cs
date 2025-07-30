using Moq;
using UserService.Application.Exceptions.IdentityServer;
using UserService.Domain.Dtos.Identity.User;
using UserService.Domain.Dtos.Token;
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

        mockIdentityServer.Setup(x => x.LoginUserAsync(It.IsAny<IdentityLoginUserDto>(), It.IsAny<CancellationToken>()))
            .Returns((IdentityLoginUserDto dto, CancellationToken _) =>
            {
                if (dto.Password == TestConstants.WrongPassword)
                    throw new IdentityServerPasswordIsWrongException("TestsIdentityServer", "Wrong password");

                return Task.FromResult(randomKeycloakUserTokenDto);
            });
        mockIdentityServer
            .Setup(x => x.RegisterUserAsync(It.IsAny<IdentityRegisterUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new IdentityUserDto(Guid.NewGuid()));
        mockIdentityServer.Setup(x => x.RefreshTokenAsync(It.IsAny<RefreshTokenDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(randomKeycloakUserTokenDto);

        return mockIdentityServer.Object;
    }
}