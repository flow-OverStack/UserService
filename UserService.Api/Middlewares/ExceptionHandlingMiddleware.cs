using System.Net.Mime;
using UserService.Application.Resources;
using UserService.Domain.Results;

namespace UserService.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next)
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

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        // We return nothing because the request is already canceled
        if (exception is OperationCanceledException) return;

        var (message, statusCode) = exception switch
        {
            _ => ($"{ErrorMessage.InternalServerError}: {exception.Message}", StatusCodes.Status500InternalServerError)
        };
        var response = BaseResult.Failure(message, statusCode);

        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.StatusCode = response.ErrorCode ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(response);
    }
}