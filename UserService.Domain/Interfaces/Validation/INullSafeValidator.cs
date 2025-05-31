namespace UserService.Domain.Interfaces.Validation;

public interface INullSafeValidator<in T>
{
    /// <summary>
    ///    Checks if instance is null and the validates it
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="errorMessages"></param>
    bool IsValid(T? instance, out IEnumerable<string> errorMessages);
}