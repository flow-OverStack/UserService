namespace UserService.Domain.Interfaces.Validation;

public interface IValidatableCredentials
{
    string Username { get; }
    string Email { get; }
}