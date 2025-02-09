using UserService.Domain.Resources;

namespace UserService.Domain.Exceptions;

public class GraphQlException(string message) : Exception(message)
{
    public static GraphQlException Internal(Exception exception)
    {
        return new GraphQlException($"{ErrorMessage.InternalServerError}: {exception.Message}");
    }
}