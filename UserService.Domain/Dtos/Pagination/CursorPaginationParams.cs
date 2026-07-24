namespace UserService.Domain.Dtos.Pagination;

public record CursorPaginationParams(int? First, string? After, string? Before, int? Last, IEnumerable<SortOrder>? Order);