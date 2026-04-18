using System.Net;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Configurations;

internal static class ExceptionHttpClientConfiguration
{
    public static HttpClient GetHttpClientConfiguration(string baseAddress)
    {
        return new HttpClient(new ExceptionThrowingHttpMessageHandler())
        {
            BaseAddress = new Uri(baseAddress)
        };
    }

    private class ExceptionThrowingHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Post && request.RequestUri?.AbsolutePath.EndsWith("/users") == true)
            {
                // Simulate a conflict response for user registration
                var response = new HttpResponseMessage(HttpStatusCode.Conflict)
                {
                    Content = new StringContent("""
                                                {
                                                    "error": "User already exists",
                                                    "error_description": "A user with the same username or email already exists."
                                                }
                                                """)
                };
                return Task.FromResult(response);
            }

            return Task.FromException<HttpResponseMessage>(new TestException());
        }
    }
}