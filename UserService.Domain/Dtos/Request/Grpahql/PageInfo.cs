namespace UserService.Domain.Dtos.Request.Grpahql;

public class PageInfo(int page, int size, int totalItems, int requestedSize)
{
    public int Page { get; } = page;

    public int Size { get; } = size;

    public int TotalItems { get; } = totalItems;

    public int TotalPages
    {
        get
        {
            if (Size != 0) return (int)Math.Ceiling(TotalItems / (double)Size);

            if (RequestedSize > 0) return (int)Math.Ceiling(TotalItems / (double)RequestedSize);

            return 0;
        }
    }

    private int RequestedSize { get; } = requestedSize;
}