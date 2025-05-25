using Microsoft.Extensions.Options;
using UserService.Application.Validators;
using UserService.Domain.Dtos.Request.Page;
using UserService.Domain.Interfaces.Validation;
using UserService.Domain.Settings;

namespace UserService.Tests.UnitTests.Configurations;

internal static class PageDtoValidatorConfiguration
{
    private static readonly BusinessRules _businessRules = BusinessRulesConfiguration.GetBusinessRules();

    public static IFallbackValidator<PageDto> GetValidator()
    {
        return new PageDtoValidator(new OptionsWrapper<BusinessRules>(_businessRules));
    }
}