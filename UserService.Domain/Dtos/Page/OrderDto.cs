using UserService.Domain.Enums;

namespace UserService.Domain.Dtos.Page;

public record OrderDto(string Field, SortDirection Direction);