using System.Net;
using Newtonsoft.Json;
using UserService.Domain.Result;
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
        // Save the original response body stream
        var originalResponseBody = httpContext.Response.Body;

        try
        {
            // Create am empty temporary memory stream to intercept the response and to read it later
            using var swapStream = new MemoryStream();
            // Redirect response output from httpContext.Response.Body to swapStream
            httpContext.Response.Body = swapStream;

            await _next(httpContext);

            swapStream.Seek(0, SeekOrigin.Begin);

            if (httpContext.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                // Read the obtained body in swapStream
                var responseBody = await new StreamReader(swapStream).ReadToEndAsync();
                swapStream.Seek(0, SeekOrigin.Begin);

                var data = JsonConvert.DeserializeObject<BaseResult>(responseBody)!; // Object means any type
                var errorMessage =
                    $"Bad request: {data.ErrorMessage!}. Path: {httpContext.Request.Path}. Method: {httpContext.Request.Method}. IP: {httpContext.Connection.RemoteIpAddress}.";

                _logger.Warning(errorMessage);
            }

            // Copy the intercepted response back to the original response stream
            await swapStream.CopyToAsync(originalResponseBody);
        }
        finally
        {
            httpContext.Response.Body = originalResponseBody;
        }
    }
}