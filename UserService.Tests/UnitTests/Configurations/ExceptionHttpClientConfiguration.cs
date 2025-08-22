using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Configurations;

internal static class ExceptionHttpClientConfiguration
{
    public static HttpClient GetHttpClientConfiguration()
    {
        return new HttpClient(new ExceptionThrowingHttpMessageHandler());
    }

    private class ExceptionThrowingHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromException<HttpResponseMessage>(new TestException());
        }
    }
}