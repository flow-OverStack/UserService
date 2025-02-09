namespace UserService.Domain.Result;

public class CollectionResult<T> : BaseResult<IEnumerable<T>>
{
    public int Count { get; set; }

    public int TotalCount { get; set; }

    public static CollectionResult<T> Success(IEnumerable<T> data, int count, int? totalCount = null)
    {
        return new CollectionResult<T> { Data = data, Count = count, TotalCount = totalCount ?? count };
    }

    public new static CollectionResult<T> Failure(string errorMessage, int? errorCode = null)
    {
        return new CollectionResult<T> { ErrorMessage = errorMessage, ErrorCode = errorCode };
    }
}