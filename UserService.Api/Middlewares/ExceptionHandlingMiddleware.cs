using System.Net.Mime;
using UserService.Application.Exceptions.IdentityServer;
using UserService.Application.Resources;
using UserService.Domain.Results;
using ILogger = Serilog.ILogger;

namespace UserService.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        logger.Error(exception, "Error: {ErrorMessage}. Path: {Path}. Method: {Method}. IP: {IP}",
            exception.Message.TrimEnd('.'),
            httpContext.Request.Path, httpContext.Request.Method, httpContext.Connection.RemoteIpAddress);

        // We return nothing because the request is already canceled 
        if (exception is OperationCanceledException) return;

        var (message, statusCode) = exception switch
        {
            IdentityServerInternalException => ($"{ErrorMessage.IdentityServerError}: {exception.Message}",
                StatusCodes.Status500InternalServerError),

            _ => ($"{ErrorMessage.InternalServerError}: {exception.Message}", StatusCodes.Status500InternalServerError)
        };
        var response = BaseResult.Failure(message, statusCode);

        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.StatusCode = response.ErrorCode ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(response);
    }
}