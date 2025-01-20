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
using UserService.Domain.Extensions;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Keycloak;
using UserService.Domain.Settings;

namespace UserService.Keycloak;

public class KeycloakServer(IOptions<KeycloakSettings> keycloakSettings) : IIdentityServer
{
    private const string IdentityServerName = "Keycloak";

    private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);
    private readonly HttpClient _httpClient = new();
    private readonly KeycloakSettings _keycloakSettings = keycloakSettings.Value;
    private static KeycloakServiceTokenDto? Token { get; set; }

    public async Task<KeycloakUserDto> RegisterUserAsync(KeycloakRegisterUserDto dto)
    {
        try
        {
            await LoginAsServiceIfNeeded();

            #region Create register request

            var userPayload = new RegisterUserPayload
            {
                Username = dto.Username,
                Email = dto.Email,

                Credentials = new List<KeycloakCredentials>().AddPassword(dto.Password),
                Attributes = new KeycloakAttributes().AddUserId(_keycloakSettings.UserIdAttributeName, dto.Id)
                    .AddRoles(_keycloakSettings.RolesAttributeName, dto.Roles)
            };

            #endregion

            #region Create user

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, Token!.AccessToken);

            var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var createResponse = await _httpClient.PostAsync(_keycloakSettings.UsersUrl, content);

            createResponse.EnsureSuccessStatusCode();

            #endregion

            #region Get created user

            var getResponse = await _httpClient.GetAsync($"{_keycloakSettings.UsersUrl}?username={dto.Username}");

            getResponse.EnsureSuccessStatusCode();

            var body = await createResponse.Content.ReadAsStringAsync();
            var responseUsers = JsonConvert.DeserializeObject<KeycloakUser[]>(body);
            var exactUser = responseUsers!.FirstOrDefault(x => x.Username == dto.Username);

            #endregion

            #region Return KeycloakUserId

            return new KeycloakUserDto(Guid.Parse(exactUser!.Id));

            #endregion
        }
        catch (Exception e)
        {
            throw new IdentityServerException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<KeycloakUserTokenDto> LoginUserAsync(KeycloakLoginUserDto dto)
    {
        try
        {
            const string grantType = "password";
            var parameters = new Dictionary<string, string>
            {
                { "client_id", _keycloakSettings.ClientId },
                { "client_secret", _keycloakSettings.AdminToken },
                { "grant_type", grantType },
                { "username", dto.Username },
                { "password", dto.Password }
            };

            var content = new FormUrlEncodedContent(parameters);

            var response = await _httpClient.PostAsync(_keycloakSettings.LoginUrl, content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var responseToken = JsonConvert.DeserializeObject<UserTokenResponse>(body);

            return new KeycloakUserTokenDto
            {
                AccessToken = responseToken!.AccessToken,
                RefreshToken = responseToken.RefreshToken,
                AccessExpires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn),
                RefreshExpires = DateTime.UtcNow.AddSeconds(responseToken.RefreshExpiresIn)
            };
        }
        catch (Exception e)
        {
            throw new IdentityServerException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<KeycloakUserTokenDto> RefreshTokenAsync(KeycloakRefreshTokenDto dto)
    {
        try
        {
            const string grantType = "refresh_token";

            var parameters = new Dictionary<string, string>
            {
                { "client_id", _keycloakSettings.ClientId },
                { "client_secret", _keycloakSettings.AdminToken },
                { "grant_type", grantType },
                { "refresh_token", dto.RefreshToken }
            };

            var content = new FormUrlEncodedContent(parameters);

            var response = await _httpClient.PostAsync(_keycloakSettings.LoginUrl, content);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var responseToken = JsonConvert.DeserializeObject<UserTokenResponse>(body);

            return new KeycloakUserTokenDto
            {
                AccessToken = responseToken!.AccessToken,
                RefreshToken = responseToken.RefreshToken,
                AccessExpires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn),
                RefreshExpires = DateTime.UtcNow.AddSeconds(responseToken.RefreshExpiresIn)
            };
        }
        catch (Exception e)
        {
            throw new IdentityServerException(IdentityServerName, e.Message, e);
        }
    }

    public async Task UpdateRolesAsync(KeycloakUpdateRolesDto dto)
    {
        try
        {
            await LoginAsServiceIfNeeded();

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

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, Token!.AccessToken);

            var response = await _httpClient.PutAsync($"{_keycloakSettings.UsersUrl}/{dto.KeycloakUserId.ToString()}",
                content);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            throw new IdentityServerException(IdentityServerName, e.Message, e);
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
        catch (Exception e)
        {
            throw new IdentityServerException(IdentityServerName, e.Message, e);
        }
    }


    private async Task LoginAsService()
    {
        const string grantType = "client_credentials";

        var parameters = new Dictionary<string, string>
        {
            { "client_id", _keycloakSettings.ClientId },
            { "client_secret", _keycloakSettings.AdminToken },
            { "grant_type", grantType }
        };

        var content = new FormUrlEncodedContent(parameters);

        var response = await _httpClient.PostAsync(_keycloakSettings.LoginUrl, content);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        var responseToken = JsonConvert.DeserializeObject<ServiceTokenResponse>(body);

        Token = new KeycloakServiceTokenDto
        {
            AccessToken = responseToken!.AccessToken,
            Expires = DateTime.UtcNow.AddSeconds(responseToken.ExpiresIn)
        };
    }

    private async Task LoginAsServiceIfNeeded()
    {
        await TokenSemaphore.WaitAsync();
        try
        {
            if (Token == null ||
                Token.Expires <= DateTime.UtcNow)
                await LoginAsService();
        }
        finally
        {
            TokenSemaphore.Release();
        }
    }

    #region Classes for http requests

    private sealed class KeycloakServiceTokenDto
    {
        public string AccessToken { get; set; }

        public DateTime Expires { get; set; }
    }

    private sealed class ServiceTokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }
        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
    }

    private sealed class UserTokenResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }
        [JsonProperty("refresh_token")] public string RefreshToken { get; set; }
        [JsonProperty("expires_in")] public int AccessExpiresIn { get; set; }
        [JsonProperty("refresh_expires_in")] public int RefreshExpiresIn { get; set; }
    }

    private sealed class KeycloakUser
    {
        public string Id { get; }
        public string Username { get; }
    }

    private sealed class RegisterUserPayload
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public bool Enabled { get; set; } = true;

        public List<KeycloakCredentials> Credentials { get; set; }
        public KeycloakAttributes Attributes { get; set; }
    }

    private sealed class UpdateUserPayload
    {
        public string Email { get; set; }

        public KeycloakAttributes Attributes { get; set; }
    }

    #endregion
}