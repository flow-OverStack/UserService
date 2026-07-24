using UserService.Domain.Enums;

namespace UserService.Domain.Dtos.Pagination;

public record SortOrder(string Field, SortDirection Direction);