using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Dto.Token;
using UserService.Domain.Exceptions.IdentityServer;
using UserService.Domain.Exceptions.IdentityServer.Base;
using UserService.Domain.Extensions;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Keycloak;
using UserService.Domain.Settings;
using UserService.Keycloak.HttpModels;

namespace UserService.Keycloak;

public class KeycloakServer(IOptions<KeycloakSettings> keycloakSettings, IHttpClientFactory httpClientFactory)
    : IIdentityServer
{
    private const string IdentityServerName = "Keycloak";
    private const string WrongGrantErrorMessage = "invalid_grant";
    private const string PasswordGrantType = "password";
    private const string RefreshTokenGrantType = "refresh_token";
    private const string ClientCredentialsGrantType = "client_credentials";
    private const int TokenExpirationThresholdInSeconds = 5;

    private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(KeycloakServer));
    private readonly KeycloakSettings _keycloakSettings = keycloakSettings.Value;
    private static KeycloakServiceToken? Token { get; set; }


    public async Task<KeycloakUserDto> RegisterUserAsync(KeycloakRegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            #region Create register request

            var userPayload = new RegisterUserPayload
            {
                Username = dto.Username,
                Email = dto.Email,

                Credentials = new List<KeycloakCredential>().AddPassword(dto.Password),
                Attributes = new KeycloakAttributes().AddUserId(_keycloakSettings.UserIdClaim, dto.Id)
                    .AddRoles(_keycloakSettings.RolesClaim, dto.Roles)
            };

            #endregion

            #region Create user

            var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await SetAuthHeaderAsync(cancellationToken);
            var createResponse =
                await _httpClient.PostAsync(_keycloakSettings.UsersEndpoint, content, cancellationToken);

            createResponse.EnsureSuccessStatusCode();

            #endregion

            #region Get created user

            await SetAuthHeaderAsync(cancellationToken);
            var getResponse = await _httpClient.GetAsync($"{_keycloakSettings.UsersEndpoint}?username={dto.Username}",
                cancellationToken);

            getResponse.EnsureSuccessStatusCode();

            var body = await getResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseUsers = JsonConvert.DeserializeObject<KeycloakUser[]>(body);
            var exactUser = responseUsers!.FirstOrDefault(x => x.Username == dto.Username);

            #endregion

            #region Return KeycloakUserId

            return new KeycloakUserDto(Guid.Parse(exactUser!.Id));

            #endregion
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<TokenDto> LoginUserAsync(KeycloakLoginUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", _keycloakSettings.ClientId },
                { "client_secret", _keycloakSettings.AdminToken },
                { "grant_type", PasswordGrantType },
                { "username", dto.Username },
                { "password", dto.Password }
            };

            var content = new FormUrlEncodedContent(parameters);

            var response = await _httpClient.PostAsync(_keycloakSettings.LoginEndpoint, content, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body);

            if (!responseToken!.IsValid())
            {
                var errorResponse = JsonConvert.DeserializeObject<KeycloakErrorResponse>(body);

                if (errorResponse!.Error == WrongGrantErrorMessage)
                    throw new IdentityServerPasswordIsWrongException(IdentityServerName,
                        errorResponse.ErrorDescription);
            }

            response.EnsureSuccessStatusCode();

            return new TokenDto
            {
                AccessToken = responseToken.AccessToken,
                RefreshToken = responseToken.RefreshToken,
                AccessExpires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn),
                RefreshExpires = DateTime.UtcNow.AddSeconds(responseToken.RefreshExpiresIn)
            };
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<TokenDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", _keycloakSettings.ClientId },
                { "client_secret", _keycloakSettings.AdminToken },
                { "grant_type", RefreshTokenGrantType },
                { "refresh_token", dto.RefreshToken }
            };

            var content = new FormUrlEncodedContent(parameters);

            var response = await _httpClient.PostAsync(_keycloakSettings.LoginEndpoint, content, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body);

            if (!responseToken!.IsValid())
            {
                var errorResponse = JsonConvert.DeserializeObject<KeycloakErrorResponse>(body);

                if (errorResponse!.Error == WrongGrantErrorMessage)
                    throw new IdentityServerInvalidTokenException(IdentityServerName,
                        errorResponse.ErrorDescription);
            }

            response.EnsureSuccessStatusCode();


            return new TokenDto
            {
                AccessToken = responseToken.AccessToken,
                RefreshToken = responseToken.RefreshToken,
                AccessExpires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn),
                RefreshExpires = DateTime.UtcNow.AddSeconds(responseToken.RefreshExpiresIn)
            };
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task UpdateRolesAsync(KeycloakUpdateRolesDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userPayload = new UpdateUserPayload
            {
                Email = dto.Email,
                Attributes = new KeycloakAttributes().AddUserId(_keycloakSettings.UserIdClaim, dto.UserId)
                    .AddRoles(_keycloakSettings.RolesClaim, dto.NewRoles)
            };

            var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await SetAuthHeaderAsync(cancellationToken);
            var response = await _httpClient.PutAsync(
                $"{_keycloakSettings.UsersEndpoint}/{dto.KeycloakUserId.ToString()}", content, cancellationToken);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task RollbackRegistrationAsync(Guid userId)
    {
        await SetAuthHeaderAsync();
        await _httpClient.DeleteAsync($"{_keycloakSettings.UsersEndpoint}/{userId.ToString()}");
    }

    public async Task RollbackUpdateRolesAsync(KeycloakUpdateRolesDto dto)
    {
        var userPayload = new UpdateUserPayload
        {
            Email = dto.Email,
            Attributes = new KeycloakAttributes().AddUserId(_keycloakSettings.UserIdClaim, dto.UserId)
                .AddRoles(_keycloakSettings.RolesClaim, dto.NewRoles)
        };

        var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await SetAuthHeaderAsync();
        await _httpClient.PutAsync($"{_keycloakSettings.UsersEndpoint}/{dto.KeycloakUserId.ToString()}", content);
    }


    private async Task UpdateServiceTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!IsTokenExpired())
            return; //double check is here to check if 2 or more threads are updating the token at the same time after the first check

        var parameters = new Dictionary<string, string>
        {
            { "client_id", _keycloakSettings.ClientId },
            { "client_secret", _keycloakSettings.AdminToken },
            { "grant_type", ClientCredentialsGrantType }
        };

        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.PostAsync(_keycloakSettings.LoginEndpoint, content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body);

        Token = new KeycloakServiceToken
        {
            AccessToken = responseToken!.AccessToken,
            Expires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn)
        };
    }

    private async Task UpdateServiceTokenIfNeededAsync(CancellationToken cancellationToken = default)
    {
        if (IsTokenExpired())
            try
            {
                await TokenSemaphore.WaitAsync(cancellationToken);
                await UpdateServiceTokenAsync(cancellationToken);
            }
            finally
            {
                TokenSemaphore.Release();
            }
    }

    private static bool IsTokenExpired()
    {
        return Token == null ||
               Token.Expires <= DateTime.UtcNow.AddSeconds(TokenExpirationThresholdInSeconds);
    }

    private async Task SetAuthHeaderAsync(CancellationToken cancellationToken = default)
    {
        await UpdateServiceTokenIfNeededAsync(cancellationToken);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, Token!.AccessToken);
    }
}