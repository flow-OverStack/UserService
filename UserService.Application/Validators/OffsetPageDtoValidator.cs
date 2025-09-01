using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Application.Settings;
using UserService.Domain.Dtos.Page;
using UserService.Domain.Interfaces.Validation;

namespace UserService.Application.Validators;

public class OffsetPageDtoValidator : AbstractValidator<OffsetPageDto>, INullSafeValidator<OffsetPageDto>
{
    public OffsetPageDtoValidator(IOptions<PaginationRules> businessRules)
    {
        var maxPageSize = businessRules.Value.MaxPageSize;

        RuleFor(x => x.Skip).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.Take).NotNull().InclusiveBetween(0, maxPageSize + 1);
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