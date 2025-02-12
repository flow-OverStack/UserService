using HotChocolate;

namespace UserService.GraphQl.ErrorFilters;

public class PublicErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        return error.WithMessage(error.Message);
    }
}