using UserService.Domain.Result;

namespace UserService.Domain.Interfaces.Exceptions;

public interface IBaseResultProvider
{
    BaseResult GetBaseResult();
}