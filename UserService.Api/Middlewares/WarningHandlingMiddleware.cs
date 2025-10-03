using System.Globalization;
using Newtonsoft.Json;
using UserService.Application.Enums;
using UserService.Application.Resources;
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
            // Create an empty temporary memory stream to intercept the response and to read it later
            using var swapStream = new MemoryStream();
            // Redirect response output from httpContext.Response.Body to swapStream
            httpContext.Response.Body = swapStream;

            await next(httpContext);

            swapStream.Seek(0, SeekOrigin.Begin);

            if (httpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
            {
                // Read the obtained body in swapStream
                var responseBody = await new StreamReader(swapStream).ReadToEndAsync();
                swapStream.Seek(0, SeekOrigin.Begin);

                var data = JsonConvert.DeserializeObject<BaseResult>(responseBody);
                var errorMessage = GetDefaultErrorMessage(data?.ErrorCode, data?.ErrorMessage);

                logger.Warning(
                    "Bad request: {ErrorMessage}. Path: {Path}. Method: {Method}. IP: {IP}",
                    errorMessage ?? responseBody, httpContext.Request.Path, httpContext.Request.Method,
                    httpContext.Connection.RemoteIpAddress);
            }

            // Copy the intercepted response back to the original response stream
            if (CanCopy(httpContext.Response, originalResponseBody)) await swapStream.CopyToAsync(originalResponseBody);
        }
        finally
        {
            httpContext.Response.Body = originalResponseBody;
        }
    }

    private static string? GetDefaultErrorMessage(int? errorCode, string? fallbackErrorMessage)
    {
        var errorMessage = errorCode is { } code && Enum.GetName(typeof(ErrorCodes), code) is { } name
            ? ErrorMessage.ResourceManager.GetString(name, CultureInfo.DefaultThreadCurrentCulture) ??
              fallbackErrorMessage
            : fallbackErrorMessage;


        return errorMessage?.TrimEnd('.');
    }

    private static bool CanCopy(HttpResponse response, Stream originalResponseBody)
    {
        if (!originalResponseBody.CanWrite) return false;

        return response.StatusCode switch
        {
            >= 100 and < 200 or StatusCodes.Status204NoContent or StatusCodes.Status304NotModified => false,
            _ => true
        };
    }
}