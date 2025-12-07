using UserService.Domain.Dtos.Page;

namespace QuestionService.Domain.Dtos.Page;

public record CursorPageDto(int? First, string? After, string? Before, int? Last, IEnumerable<OrderDto>? Order);