using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UserService.Keycloak.HttpModels;
using UserService.Keycloak.Settings;

namespace UserService.Keycloak;

internal sealed class KeycloakAuthHandler(
    IOptions<KeycloakSettings> keycloakSettings,
    IHttpClientFactory httpClientFactory)
    : DelegatingHandler
{
    internal const string TokenClientName = "KeycloakToken";
    private const string ClientCredentialsGrantType = "client_credentials";
    private const int TokenExpirationThresholdInSeconds = 5;

    private static readonly SemaphoreSlim TokenSemaphore = new(1, 1);
    private static KeycloakServiceToken? _token;
    private readonly KeycloakSettings _settings = keycloakSettings.Value;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await EnsureFreshTokenAsync(cancellationToken);
        request.Headers.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, _token!.AccessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task EnsureFreshTokenAsync(CancellationToken cancellationToken)
    {
        if (!IsTokenExpired()) return;

        await TokenSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (!IsTokenExpired()) return;
            await FetchTokenAsync(_settings, httpClientFactory, cancellationToken);
        }
        finally
        {
            TokenSemaphore.Release();
        }
    }

    private static async Task FetchTokenAsync(KeycloakSettings settings, IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken)
    {
        if (!IsTokenExpired()) return;

        var parameters = new Dictionary<string, string>
        {
            { "client_id", settings.ClientId },
            { "client_secret", settings.AdminToken },
            { "grant_type", ClientCredentialsGrantType }
        };

        var tokenClient = httpClientFactory.CreateClient(TokenClientName);
        var content = new FormUrlEncodedContent(parameters);
        var response = await tokenClient.PostAsync(settings.LoginEndpoint, content, cancellationToken);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseToken = JsonConvert.DeserializeObject<KeycloakTokenResponse>(body)!;

        _token = new KeycloakServiceToken
        {
            AccessToken = responseToken.AccessToken,
            Expires = DateTime.UtcNow.AddSeconds(responseToken.AccessExpiresIn)
        };
    }

    private static bool IsTokenExpired()
    {
        return _token == null || _token.Expires <= DateTime.UtcNow.AddSeconds(TokenExpirationThresholdInSeconds);
    }
}