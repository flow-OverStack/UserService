using FluentValidation;
using Microsoft.Extensions.Options;
using QuestionService.Domain.Dtos.Page;
using UserService.Application.Settings;
using UserService.Domain.Extensions;
using UserService.Domain.Interfaces.Validation;

namespace UserService.Application.Validators;

public class CursorPageDtoValidator : AbstractValidator<CursorPageDto>, INullSafeValidator<CursorPageDto>
{
    public CursorPageDtoValidator(IOptions<PaginationRules> paginationRules)
    {
        RuleFor(x => x.First)
            .InclusiveBetween(0, paginationRules.Value.MaxPageSize)
            .When(x => x.First.HasValue);

        RuleFor(x => x.Last)
            .InclusiveBetween(0, paginationRules.Value.MaxPageSize)
            .When(x => x.Last.HasValue);

        RuleFor(x => x.After)
            .Must(x => x!.IsBase64())
            .When(x => x.After != null)
            .WithMessage($"'{nameof(CursorPageDto.After)}' must be a valid base64 string.");

        RuleFor(x => x.Before)
            .Must(x => x!.IsBase64())
            .When(x => x.Before != null)
            .WithMessage($"'{nameof(CursorPageDto.Before)}' must be a valid base64 string.");

        RuleFor(x => x.Order).NotEmpty()
            .ForEach(eachOrder =>
            {
                eachOrder.NotNull().ChildRules(order =>
                {
                    order.RuleFor(o => o.Field).NotEmpty();
                    order.RuleFor(o => o.Direction).NotNull().IsInEnum();
                });
            });

        var errorMessage = string.Format(
            "You must specify either '{0}' (optionally with '{1}'), without '{2}' and '{3}', " +
            "or '{2}' (optionally with '{3}'), without '{1}' and '{2}'.",
            nameof(CursorPageDto.First).LowercaseFirstLetter(), nameof(CursorPageDto.After).LowercaseFirstLetter(),
            nameof(CursorPageDto.Last).LowercaseFirstLetter(), nameof(CursorPageDto.Before).LowercaseFirstLetter());
        RuleFor(x => x)
            .Must(x =>
                x is { First: not null, Last: null, Before: null }
                    or { Last: not null, First: null, After: null })
            .WithMessage(errorMessage);
    }

    public bool IsValid(CursorPageDto? instance, out IEnumerable<string> errorMessages)
    {
        errorMessages = [];

        if (instance == null) return false;

        var result = Validate(instance);

        errorMessages = result.Errors.Select(x => x.ErrorMessage);

        return result.IsValid;
    }
}