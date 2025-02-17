using System.Net;
using UserService.Domain.Exceptions.IdentityServer;
using UserService.Domain.Resources;
using UserService.Domain.Result;
using ILogger = Serilog.ILogger;

namespace UserService.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly ILogger _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);

            switch (httpContext.Response.StatusCode)
            {
                case (int)HttpStatusCode.NotFound:
                    httpContext.Response.ContentType = "text/plain";
                    var message = $"{(int)HttpStatusCode.NotFound} {nameof(HttpStatusCode.NotFound)}\nPlease check URL";
                    await httpContext.Response.WriteAsync(message);
                    break;
            }
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        _logger.Error(exception, "Error: {errorMessage}. Path: {Path}. Method: {Method}. IP: {IP}", exception.Message,
            httpContext.Request.Path, httpContext.Request.Method, httpContext.Connection.RemoteIpAddress);


        var response = exception switch
        {
            IdentityServerInternalException => BaseResult.Failure(
                $"{ErrorMessage.IdentityServerError}: {exception.Message}", (int)HttpStatusCode.InternalServerError),

            _ => BaseResult.Failure($"{ErrorMessage.InternalServerError}: {exception.Message}",
                (int)HttpStatusCode.InternalServerError)
        };


        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)response.ErrorCode!;
        await httpContext.Response.WriteAsJsonAsync(response);
    }
}