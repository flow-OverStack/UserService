namespace UserService.Domain.Interfaces.Validation;

public interface INullSafeValidator<in T>
{
    /// <summary>
    ///     Returns the given instance if valid; otherwise returns a fallback valid instance of T.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="errorMessages"></param>
    bool IsValid(T? instance, out IEnumerable<string> errorMessages);
}