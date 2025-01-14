namespace UserService.Domain.Dto.User;

public record RegisterUserDto(string Username, string Email, string Password, string PasswordConfirm);