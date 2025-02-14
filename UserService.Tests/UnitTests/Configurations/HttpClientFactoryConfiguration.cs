using Moq;
using UserService.Tests.Configurations;

namespace UserService.Tests.UnitTests.Configurations;

internal static class HttpClientFactoryConfiguration
{
    public static IHttpClientFactory GetHttpClientFactoryConfiguration()
    {
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        var exceptionHttpClient = new HttpClient(new ExceptionThrowingHttpMessageHandler());

        mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(exceptionHttpClient);

        return mockHttpClientFactory.Object;
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