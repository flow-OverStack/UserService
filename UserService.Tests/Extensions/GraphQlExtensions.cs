using System.Net.Http.Headers;

namespace UserService.Tests.Extensions;

public static class GraphQlExtensions
{
    public static void SetGraphQlAuthHeaders(this HttpClient httpClient, string serviceName)
    {
        var serviceToken = TokenExtensions.GetServiceRsaToken(serviceName);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceToken);
    }
}