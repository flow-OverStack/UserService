using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UserService.Domain.Dto.Keycloak.Roles;
using UserService.Domain.Dto.Keycloak.Token;
using UserService.Domain.Dto.Keycloak.User;
using UserService.Domain.Exceptions.IdentityServer;
using UserService.Domain.Extensions;
using UserService.Domain.Interfaces.Services;
using UserService.Domain.Keycloak;
using UserService.Domain.Settings;

namespace UserService.Keycloak;

public class KeycloakServer : IIdentityServer
{
    private const string IdentityServerName = "Keycloak";
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _keycloakSettings;


    public KeycloakServer(IOptions<KeycloakSettings> keycloakSettings)
    {
        _httpClient = new HttpClient();
        _keycloakSettings = keycloakSettings.Value;
    }

    private static KeycloakServiceTokenDto? Token { get; set; }


    public async Task<KeycloakUserDto> RegisterUserAsync(KeycloakRegisterUserDto dto)
    {
        try
        {
            await LoginAsServiceIfNeeded();

            #region Create register request

            var registerUserRequest = new RegisterUserRequest
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

            var json = JsonConvert.SerializeObject(registerUserRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_keycloakSettings.UsersUrl, content);

            response.EnsureSuccessStatusCode();

            #endregion

            #region Get created user

            response = await _httpClient.GetAsync($"{_keycloakSettings.UsersUrl}?username={dto.Username}");

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
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
                Expires = DateTime.UtcNow.AddSeconds(responseToken.ExpiresIn)
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
                Expires = DateTime.UtcNow.AddSeconds(responseToken.ExpiresIn)
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

            var json = JsonConvert.SerializeObject(
                new KeycloakAttributes().AddRoles(_keycloakSettings.RolesAttributeName, dto.newRoles));
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_keycloakSettings.UsersUrl}/{dto.keycloakUserId.ToString()}",
                content);

            response.EnsureSuccessStatusCode();
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
        if (Token == null ||
            Token.Expires <= DateTime.UtcNow)
            await LoginAsService();
    }

    #region Classes for http requests

    private sealed class KeycloakServiceTokenDto
    {
        public string AccessToken { get; set; }

        public DateTime Expires { get; set; }
    }

    private sealed class ServiceTokenResponse
    {
        public string AccessToken { get; }
        public int ExpiresIn { get; }
    }

    private sealed class UserTokenResponse
    {
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public int ExpiresIn { get; }
    }

    private sealed class KeycloakUser
    {
        public string Id { get; }
        public string Username { get; }
    }

    private sealed class RegisterUserRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public bool Enabled { get; set; } = true;

        public List<KeycloakCredentials> Credentials { get; set; }
        public KeycloakAttributes Attributes { get; set; }
    }

    #endregion
}