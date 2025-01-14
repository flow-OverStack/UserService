using System.Net;
using Newtonsoft.Json;
using ILogger = Serilog.ILogger;

namespace UserService.Api.Middlewares;

public class WarningHandlingMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public WarningHandlingMiddleware(ILogger logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var originalResponseBody = httpContext.Response.Body;

        try
        {
            using (var swapStream = new MemoryStream())
            {
                httpContext.Response.Body = swapStream;

                await _next(httpContext);

                swapStream.Seek(0, SeekOrigin.Begin);

                if (httpContext.Response.StatusCode == (int)HttpStatusCode.BadRequest)
                {
                    var responseBody = await new StreamReader(swapStream).ReadToEndAsync();
                    swapStream.Seek(0, SeekOrigin.Begin);
                    dynamic data = JsonConvert.DeserializeObject(responseBody)!;
                    string errorMessage = data.errorMessage;
                    _logger.Warning(new ArgumentException("Bad request: " + errorMessage), errorMessage);
                }

                await swapStream.CopyToAsync(originalResponseBody);
            }
        }
        finally
        {
            httpContext.Response.Body = originalResponseBody;
        }
    }
}