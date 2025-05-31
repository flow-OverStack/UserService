using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Interfaces.Validation;
using UserService.Domain.Settings;

namespace UserService.Application.Validators;

public class OffsetPageDtoValidator : AbstractValidator<OffsetPageDto>, INullSafeValidator<OffsetPageDto>
{
    public OffsetPageDtoValidator(IOptions<BusinessRules> businessRules)
    {
        var maxPageSize = businessRules.Value.MaxPageSize;

        RuleFor(x => x.Skip).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.Take).NotNull().InclusiveBetween(0, maxPageSize);
    }

    public bool IsValid(OffsetPageDto? instance, out IEnumerable<string> errorMessages)
    {
        errorMessages = [];

        if (instance == null) return false;

        var result = Validate(instance);

        errorMessages = result.Errors.Select(x => x.ErrorMessage);

        return result.IsValid;
    }
}