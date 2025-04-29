using System.Net;
using Newtonsoft.Json;
using UserService.Domain.Results;
using ILogger = Serilog.ILogger;

namespace UserService.Api.Middlewares;

public class WarningHandlingMiddleware(ILogger logger, RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Checks if request is gRPC
        if (httpContext.Request.ContentType?.StartsWith("application/grpc") == true)
        {
            await next(httpContext);
            return;
        }

        // Save the original response body stream
        var originalResponseBody = httpContext.Response.Body;

        try
        {
            // Create am empty temporary memory stream to intercept the response and to read it later
            using var swapStream = new MemoryStream();
            // Redirect response output from httpContext.Response.Body to swapStream
            httpContext.Response.Body = swapStream;

            await next(httpContext);

            swapStream.Seek(0, SeekOrigin.Begin);

            if (httpContext.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                // Read the obtained body in swapStream
                var responseBody = await new StreamReader(swapStream).ReadToEndAsync();
                swapStream.Seek(0, SeekOrigin.Begin);

                var data = JsonConvert.DeserializeObject<BaseResult>(responseBody)!;

                logger.Warning("Bad request: {errorMessage}. Path: {Path}. Method: {Method}. IP: {IP}",
                    data.ErrorMessage!,
                    httpContext.Request.Path, httpContext.Request.Method, httpContext.Connection.RemoteIpAddress);
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