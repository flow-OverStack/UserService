using FluentValidation;

namespace UserService.Application.Helpers;

public static class ValidationExtensions
{
    /// <summary>
    ///     Validates the specified value and returns a combined error message if validation fails.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="validator">The validator instance.</param>
    /// <param name="value">The value to validate.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A tuple where <c>IsValid</c> indicates whether validation passed,
    ///     and <c>ErrorMessage</c> contains all validation errors joined into a single string,
    ///     or <see cref="string.Empty" /> if validation succeeded.
    /// </returns>
    public static async Task<(bool IsValid, string ErrorMessage)> ValidateWithMessageAsync<T>(
        this IValidator<T> validator, T value, CancellationToken cancellationToken = default)
    {
        var result = await validator.ValidateAsync(value, cancellationToken);
        if (result.IsValid) return (true, string.Empty);

        var errorMessage = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
        return (false, errorMessage);
    }
}