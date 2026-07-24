namespace UserService.Domain.Dtos.Pagination;

public record OffsetPaginationParams(int? Skip, int? Take);