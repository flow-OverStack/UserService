using Microsoft.IdentityModel.Tokens;
using Moq;
using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Exceptions.IdentityServer;
using UserService.Domain.Interfaces.Services;
using UserService.Tests.Extensions;

namespace UserService.Tests.UnitTests.Configurations;

internal static class IdentityServerConfiguration
{
    public static IIdentityServer GetIdentityServerConfiguration()
    {
        var mockIdentityServer = new Mock<IIdentityServer>();

        var randomKeycloakUserTokenDto = new KeycloakUserTokenDto
        {
            AccessToken = "newAccessToken",
            AccessExpires = DateTime.UtcNow.AddSeconds(300), //random value
            RefreshToken = "newRefreshToken",
            RefreshExpires = DateTime.UtcNow.AddMinutes(30) //random value
        };

        mockIdentityServer.Setup(x => x.LoginUserAsync(It.IsAny<KeycloakLoginUserDto>()))
            .Returns((KeycloakLoginUserDto dto) =>
            {
                if (dto.Password == TestConstants.WrongPassword)
                    throw new IdentityServerPasswordIsWrongException("TestsIdentityServer", "Wrong password");

                return Task.FromResult(randomKeycloakUserTokenDto);
            });
        mockIdentityServer.Setup(x => x.RegisterUserAsync(It.IsAny<KeycloakRegisterUserDto>())).ReturnsAsync(
            new KeycloakUserDto(Guid.NewGuid()));
        mockIdentityServer.Setup(x => x.RefreshTokenAsync(It.IsAny<KeycloakRefreshTokenDto>()))
            .ReturnsAsync(randomKeycloakUserTokenDto);
        mockIdentityServer.Setup(x => x.GetTokenValidationParametersAsync()).ReturnsAsync(new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = TokenExtensions.GetPublicSigningKey(),
            ValidateIssuer = true,
            ValidIssuer = TokenExtensions.GetIssuer(),
            ValidateAudience = true,
            ValidAudiences = [TokenExtensions.GetAudience(), TokenExtensions.GetServiceAudience()],
            ValidateLifetime = false
        });
        mockIdentityServer.Setup(x => x.UpdateRolesAsync(It.IsAny<KeycloakUpdateRolesDto>()));

        return mockIdentityServer.Object;
    }
}