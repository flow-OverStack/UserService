using FluentValidation;

namespace UserService.Tests.UnitTests.Fixtures;

internal static class ValidatorFixture<T>
{
    public static IValidator<T> GetValidator(AbstractValidator<T> validator)
    {
        validator.RuleLevelCascadeMode = CascadeMode.Stop;
        return validator;
    }
}