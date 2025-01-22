using Microsoft.IdentityModel.Tokens;
using Moq;
using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Interfaces.Services;
using UserService.Tests.Extensions;

namespace UserService.Tests.UnitTests.Configurations;

public static class IdentityServerConfiguration
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
            .ReturnsAsync(randomKeycloakUserTokenDto);
        mockIdentityServer.Setup(x => x.RegisterUserAsync(It.IsAny<KeycloakRegisterUserDto>())).ReturnsAsync(
            new KeycloakUserDto(Guid.NewGuid()));
        mockIdentityServer.Setup(x => x.RefreshTokenAsync(It.IsAny<KeycloakRefreshTokenDto>()))
            .ReturnsAsync(randomKeycloakUserTokenDto);
        mockIdentityServer.Setup(x => x.GetTokenValidationParametersAsync()).ReturnsAsync(new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningKeyExtensions.GetPublicSigningKey(),
            ValidateIssuer = true,
            ValidIssuer = SigningKeyExtensions.GetIssuer(),
            ValidateAudience = true,
            ValidAudience = SigningKeyExtensions.GetAudience(),
            ValidateLifetime = false
        });
        mockIdentityServer.Setup(x => x.UpdateRolesAsync(It.IsAny<KeycloakUpdateRolesDto>()));

        return mockIdentityServer.Object;
    }
}