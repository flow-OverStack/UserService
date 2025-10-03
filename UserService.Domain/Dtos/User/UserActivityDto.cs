namespace UserService.Domain.Dtos.User;

public record UserActivityDto(long UserId, DateTime LastLoginAt);