using UserService.Domain.Result;

namespace Diary.Domain.Result;

public class CollectionResult<T> : BaseResult<IEnumerable<T>>
{
    public int Count { get; set; }

    public int TotalCount { get; set; }
}