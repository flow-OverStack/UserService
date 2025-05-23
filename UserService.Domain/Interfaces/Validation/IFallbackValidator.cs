namespace UserService.Domain.Interfaces.Validation;

public interface IFallbackValidator<T>
{
    /// <summary>
    ///     Returns the given instance if valid; otherwise returns a fallback valid instance of T.
    /// </summary>
    T GetOrFallback(T instance);
}