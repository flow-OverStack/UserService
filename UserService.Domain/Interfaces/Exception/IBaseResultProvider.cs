using UserService.Domain.Results;

namespace UserService.Domain.Interfaces.Exception;

public interface IBaseResultProvider
{
    BaseResult GetBaseResult();
}