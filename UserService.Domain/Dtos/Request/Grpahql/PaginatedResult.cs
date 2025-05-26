using System.Diagnostics.CodeAnalysis;
using UserService.Domain.Results;

namespace UserService.Domain.Dtos.Request.Grpahql;

public class PaginatedResult<T> where T : class
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public PaginatedResult(PageResult<T> pageResult, int requestedPageSize)
    {
        if (!pageResult.IsSuccess)
            throw new ArgumentException("Page result is unsuccessful.", nameof(pageResult));

        PageInfo = new PageInfo(pageResult.PageNumber, pageResult.Count, pageResult.TotalCount, requestedPageSize);
        Items = pageResult.Data;
    }

    public PageInfo PageInfo { get; }
    public IEnumerable<T> Items { get; }
}