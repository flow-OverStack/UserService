using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Keycloak.User;
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
    private const string WrongPasswordErrorMessage = "invalid_grant";
    private const string PasswordGrantType = "password";
    private const string RefreshTokenGrantType = "refresh_token";
    private const string ClientCredentialsGrantType = "client_credentials";
    private const int TokenExpirationThresholdInSeconds = 5;

    private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("KeycloakHttpClient");
    private readonly KeycloakSettings _keycloakSettings = keycloakSettings.Value;
    private static KeycloakServiceToken? Token { get; set; }


    public async Task<KeycloakUserDto> RegisterUserAsync(KeycloakRegisterUserDto dto)
    {
        try
        {
            #region Create register request

            var userPayload = new RegisterUserPayload
            {
                Username = dto.Username,
                Email = dto.Email,

                Credentials = new List<KeycloakCredential>().AddPassword(dto.Password),
                Attributes = new KeycloakAttributes().AddUserId(_keycloakSettings.UserIdAttributeName, dto.Id)
                    .AddRoles(_keycloakSettings.RolesAttributeName, dto.Roles)
            };

            #endregion

            #region Create user

            var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await SetAuthHeader();
            var createResponse = await _httpClient.PostAsync(_keycloakSettings.UsersUrl, content);

            createResponse.EnsureSuccessStatusCode();

            #endregion

            #region Get created user

            await SetAuthHeader();
            var getResponse = await _httpClient.GetAsync($"{_keycloakSettings.UsersUrl}?username={dto.Username}");

            getResponse.EnsureSuccessStatusCode();

            var body = await getResponse.Content.ReadAsStringAsync();
            var responseUsers = JsonConvert.DeserializeObject<KeycloakUser[]>(body);
            var exactUser = responseUsers!.FirstOrDefault(x => x.Username == dto.Username);

            #endregion

            #region Return KeycloakUserId

            return new KeycloakUserDto(Guid.Parse(exactUser!.Id));

            #endregion
        }
        catch (Exception e) when (e is not IdentityServerBusinessException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<KeycloakUserTokenDto> LoginUserAsync(KeycloakLoginUserDto dto)
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

            var response = await _httpClient.PostAsync(_keycloakSettings.LoginUrl, content);

            var body = await response.Content.ReadAsStringAsync();
            var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body);

            if (!responseToken!.IsValid())
            {
                var errorResponse = JsonConvert.DeserializeObject<KeycloakErrorResponse>(body);

                if (errorResponse!.Error == WrongPasswordErrorMessage)
                    throw new IdentityServerPasswordIsWrongException(IdentityServerName,
                        errorResponse.ErrorDescription);
            }

            response.EnsureSuccessStatusCode();

            return new KeycloakUserTokenDto
            {
                AccessToken = responseToken.AccessToken,
                RefreshToken = responseToken.RefreshToken,
                AccessExpires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn),
                RefreshExpires = DateTime.UtcNow.AddSeconds(responseToken.RefreshExpiresIn)
            };
        }
        catch (Exception e) when (e is not IdentityServerBusinessException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<KeycloakUserTokenDto> RefreshTokenAsync(KeycloakRefreshTokenDto dto)
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

            var response = await _httpClient.PostAsync(_keycloakSettings.LoginUrl, content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body);

            return new KeycloakUserTokenDto
            {
                AccessToken = responseToken!.AccessToken,
                RefreshToken = responseToken.RefreshToken,
                AccessExpires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn),
                RefreshExpires = DateTime.UtcNow.AddSeconds(responseToken.RefreshExpiresIn)
            };
        }
        catch (Exception e) when (e is not IdentityServerBusinessException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task UpdateRolesAsync(KeycloakUpdateRolesDto dto)
    {
        try
        {
            var userPayload = new UpdateUserPayload
            {
                Email = dto.Email,
                Attributes = new KeycloakAttributes().AddUserId(_keycloakSettings.UserIdAttributeName, dto.UserId)
                    .AddRoles(_keycloakSettings.RolesAttributeName, dto.NewRoles)
            };

            var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await SetAuthHeader();
            var response = await _httpClient.PutAsync($"{_keycloakSettings.UsersUrl}/{dto.KeycloakUserId.ToString()}",
                content);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception e) when (e is not IdentityServerBusinessException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<TokenValidationParameters> GetTokenValidationParametersAsync()
    {
        try
        {
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                _keycloakSettings.MetadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever
                {
                    RequireHttps = false
                });

            var openIdConfiguration = await configurationManager.GetConfigurationAsync();


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = openIdConfiguration.SigningKeys,
                ValidateLifetime = false,
                ValidAudience = _keycloakSettings.Audience,
                ValidIssuer = openIdConfiguration.Issuer
            };


            return tokenValidationParameters;
        }
        catch (Exception e) when (e is not IdentityServerBusinessException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task RollbackRegistration(Guid userId)
    {
        await SetAuthHeader();
        await _httpClient.DeleteAsync($"{_keycloakSettings.UsersUrl}/{userId.ToString()}");
    }

    public async Task RollbackUpdateRolesAsync(KeycloakUpdateRolesDto dto)
    {
        var userPayload = new UpdateUserPayload
        {
            Email = dto.Email,
            Attributes = new KeycloakAttributes().AddUserId(_keycloakSettings.UserIdAttributeName, dto.UserId)
                .AddRoles(_keycloakSettings.RolesAttributeName, dto.NewRoles)
        };

        var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        await SetAuthHeader();
        await _httpClient.PutAsync($"{_keycloakSettings.UsersUrl}/{dto.KeycloakUserId.ToString()}",
            content);
    }


    private async Task UpdateServiceToken()
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

        var response = await _httpClient.PostAsync(_keycloakSettings.LoginUrl, content);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body);

        Token = new KeycloakServiceToken
        {
            AccessToken = responseToken!.AccessToken,
            Expires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn)
        };
    }

    private async Task UpdateServiceTokenIfNeeded()
    {
        if (IsTokenExpired())
            try
            {
                await TokenSemaphore.WaitAsync();
                await UpdateServiceToken();
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

    private async Task SetAuthHeader()
    {
        await UpdateServiceTokenIfNeeded();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, Token!.AccessToken);
    }
}