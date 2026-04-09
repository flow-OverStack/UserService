using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserService.Application.Exceptions.IdentityServer;
using UserService.Application.Exceptions.IdentityServer.Base;
using UserService.Domain.Dtos.Identity;
using UserService.Domain.Dtos.Token;
using UserService.Domain.Interfaces.Identity;
using UserService.Keycloak.Extensions;
using UserService.Keycloak.HttpModels;
using UserService.Keycloak.KeycloakModels;
using UserService.Keycloak.Settings;

namespace UserService.Keycloak;

public class KeycloakServer(IOptions<KeycloakSettings> keycloakSettings, HttpClient httpClient, IMapper mapper)
    : IIdentityServer
{
    private const string IdentityServerName = "Keycloak";
    private const string WrongGrantErrorMessage = "invalid_grant";
    private const string PasswordGrantType = "password";
    private const string RefreshTokenGrantType = "refresh_token";

    private readonly KeycloakSettings _keycloakSettings = keycloakSettings.Value;

    public async Task<IdentityUserDto> RegisterUserAsync(IdentityRegisterUserDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userPayload = new RegisterUserPayload
            {
                Username = dto.Username,
                Email = dto.Email,
                Credentials = new List<KeycloakCredential>().AddPassword(dto.Password),
                Attributes = new KeycloakAttributes()
                    .AddUserId(_keycloakSettings.UserIdClaim, dto.Id)
                    .AddRoles(_keycloakSettings.RolesClaim, dto.Roles)
            };

            var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            // CancellationToken.None: once the request is sent we must know the outcome.
            // Cancelling here would leave Keycloak in an unknown state (user may or may not be created).
            var createResponse =
                await httpClient.PostAsync(_keycloakSettings.UsersEndpoint, content, CancellationToken.None);

            if (createResponse.StatusCode == HttpStatusCode.Conflict)
            {
                var errorBody = await createResponse.Content.ReadAsStringAsync(CancellationToken.None);
                var errorResponse = JsonConvert.DeserializeObject<KeycloakErrorResponse>(errorBody);

                throw new IdentityServerInvalidTokenException(IdentityServerName,
                    $"User with the same username or email already exists. {errorResponse?.ErrorDescription}");
            }

            createResponse.EnsureSuccessStatusCode();

            var getResponse = await httpClient.GetAsync(
                $"{_keycloakSettings.UsersEndpoint}?username={Uri.EscapeDataString(dto.Username)}",
                cancellationToken);

            getResponse.EnsureSuccessStatusCode();

            var body = await getResponse.Content.ReadAsStringAsync(cancellationToken);
            var responseUsers = JsonConvert.DeserializeObject<KeycloakUser[]>(body);
            var exactUser = responseUsers!.FirstOrDefault(x => x.Username == dto.Username);

            return mapper.Map<IdentityUserDto>(exactUser);
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<IdentityUserDto?> FindUserAsync(string identifier,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return IsEmail(identifier)
                ? await FindByEmailAsync(identifier, cancellationToken)
                : await FindByUsernameAsync(identifier, cancellationToken);
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task<TokenDto> LoginUserAsync(IdentityLoginUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "client_id", _keycloakSettings.ClientId },
                { "client_secret", _keycloakSettings.AdminToken },
                { "grant_type", PasswordGrantType },
                { "username", dto.Identifier },
                { "password", dto.Password }
            };

            var content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync(_keycloakSettings.LoginEndpoint, content, cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body);

            if (!responseToken!.IsValid())
            {
                var errorResponse = JsonConvert.DeserializeObject<KeycloakErrorResponse>(body);

                if (errorResponse!.Error == WrongGrantErrorMessage)
                    throw new IdentityServerInvalidCredentialsException(IdentityServerName,
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
            var response = await httpClient.PostAsync(_keycloakSettings.LoginEndpoint, content, cancellationToken);

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

    public async Task UpdateUserAsync(IdentityUpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await SendUpdateUserAsync(dto, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e) when (e is not IdentityServerBusinessException && e is not OperationCanceledException)
        {
            throw new IdentityServerInternalException(IdentityServerName, e.Message, e);
        }
    }

    public async Task DeleteUserAsync(IdentityUserIdDto dto)
    {
        var response = await httpClient.DeleteAsync($"{_keycloakSettings.UsersEndpoint}/{dto.IdentityId}",
            CancellationToken.None);

        // 404 means the user is already gone — that is the desired end state.
        // Any other non-success code is a real error: throw so Hangfire retries the job.
        if (response.StatusCode != HttpStatusCode.NotFound)
            response.EnsureSuccessStatusCode();
    }

    private async Task<IdentityUserDto?> FindByUsernameAsync(string username,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"{_keycloakSettings.UsersEndpoint}?username={Uri.EscapeDataString(username)}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var users = JsonConvert.DeserializeObject<KeycloakUser[]>(body);

        // Keycloak search is prefix-based — filter for exact match.
        var exactUser = users!.FirstOrDefault(x => x.Username == username);
        return mapper.Map<IdentityUserDto>(exactUser);
    }

    private async Task<IdentityUserDto?> FindByEmailAsync(string email,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"{_keycloakSettings.UsersEndpoint}?email={Uri.EscapeDataString(email)}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var users = JsonConvert.DeserializeObject<KeycloakUser[]>(body);

        var exactUser = users!.FirstOrDefault(x => x.Email == email);
        return mapper.Map<IdentityUserDto>(exactUser);
    }

    private Task<HttpResponseMessage> SendUpdateUserAsync(IdentityUpdateUserDto dto,
        CancellationToken cancellationToken = default)
    {
        var userPayload = new UpdateUserPayload
        {
            Username = dto.Username,
            Email = dto.Email,
            Attributes = new KeycloakAttributes()
                .AddUserId(_keycloakSettings.UserIdClaim, dto.UserId)
                .AddRoles(_keycloakSettings.RolesClaim, dto.Roles)
        };

        var json = JsonConvert.SerializeObject(userPayload, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        return httpClient.PutAsync($"{_keycloakSettings.UsersEndpoint}/{dto.IdentityId}", content, cancellationToken);
    }

    private static bool IsEmail(string identifier) => MailAddress.TryCreate(identifier, out _);
}