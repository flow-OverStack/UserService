using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Interfaces.Validation;
using UserService.Domain.Settings;

namespace UserService.Application.Validators;

public class PageDtoValidator : AbstractValidator<PageDto>, INullSafeValidator<PageDto>
{
    public PageDtoValidator(IOptions<BusinessRules> businessRules)
    {
        var maxPageSize = businessRules.Value.MaxPageSize;

        RuleFor(x => x.Skip).NotNull().GreaterThanOrEqualTo(0);
        RuleFor(x => x.Take).NotNull().InclusiveBetween(1, maxPageSize);
    }

    public bool IsValid(PageDto? instance, out IEnumerable<string> errorMessages)
    {
        errorMessages = [];

        if (instance == null) return false;

        var result = Validate(instance);

        errorMessages = result.Errors.Select(x => x.ErrorMessage);

        return result.IsValid;
    }
}