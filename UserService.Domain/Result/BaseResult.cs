namespace UserService.Domain.Result;

public class BaseResult
{
    public bool IsSuccess => ErrorMessage == null;

    public string? ErrorMessage { get; set; }

    public int? ErrorCode { get; set; }
}

public class BaseResult<T> : BaseResult
{
    public T Data { get; set; }
}