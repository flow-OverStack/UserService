using HotChocolate;
using UserService.Domain.Resources;

namespace UserService.Domain.Helpers;

public static class GraphQlExceptionHelper
{
    public static GraphQLException GetInternal(Exception exception)
    {
        return new GraphQLException(ErrorBuilder.New()
            .SetMessage($"{ErrorMessage.InternalServerError}: {exception.Message}")
            .Build());
    }

    public static GraphQLException Get(string errorMessage)
    {
        return new GraphQLException(ErrorBuilder.New()
            .SetMessage(errorMessage)
            .Build());
    }
}