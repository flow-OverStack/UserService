namespace UserService.Domain.Results;

public class BaseResult
{
    public bool IsSuccess => ErrorMessage == null;

    public string? ErrorMessage { get; set; }

    public int? ErrorCode { get; set; }

    public static BaseResult Success()
    {
        return new BaseResult();
    }

    public static BaseResult Failure(string errorMessage, int? errorCode = null)
    {
        return new BaseResult { ErrorMessage = errorMessage, ErrorCode = errorCode };
    }
}

public class BaseResult<T> : BaseResult
{
    public T Data { get; set; }

    public static BaseResult<T> Success(T data)
    {
        return new BaseResult<T> { Data = data };
    }

    public new static BaseResult<T> Failure(string errorMessage, int? errorCode = null)
    {
        return new BaseResult<T> { ErrorMessage = errorMessage, ErrorCode = errorCode };
    }
}