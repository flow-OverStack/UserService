using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Interfaces.Validation;
using UserService.Domain.Settings;

namespace UserService.Application.Validators;

public class PageDtoValidator
    : AbstractValidator<PageDto>, IFallbackValidator<PageDto>
{
    private readonly int _maxPageSize;

    public PageDtoValidator(IOptions<BusinessRules> businessRules)
    {
        _maxPageSize = businessRules.Value.MaxPageSize;

        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }

    public PageDto GetOrFallback(PageDto instance)
    {
        return IsValid(instance) ? instance : new PageDto(1, _maxPageSize);
    }

    private bool IsValid(PageDto? instance)
    {
        return instance != null && Validate(instance).IsValid;
    }
}