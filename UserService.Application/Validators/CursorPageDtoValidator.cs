using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Application.Settings;
using UserService.Domain.Dtos.Page;
using UserService.Domain.Extensions;

namespace UserService.Application.Validators;

public class CursorPageDtoValidator : AbstractValidator<CursorPageDto>
{
    public CursorPageDtoValidator(IOptions<PaginationRules> pagination)
    {
        RuleFor(x => x.First)
            .InclusiveBetween(0, pagination.Value.MaxPageSize)
            .WithMessage($"'{nameof(CursorPageDto.First)}' must be between 0 and {pagination.Value.MaxPageSize}.")
            .When(x => x.First.HasValue);

        RuleFor(x => x.Last)
            .InclusiveBetween(0, pagination.Value.MaxPageSize)
            .WithMessage($"'{nameof(CursorPageDto.Last)}' must be between 0 and {pagination.Value.MaxPageSize}.")
            .When(x => x.Last.HasValue);

        RuleFor(x => x.After)
            .Must(x => x!.IsBase64())
            .WithMessage($"'{nameof(CursorPageDto.After)}' must be a valid base64 string.")
            .When(x => x.After != null);

        RuleFor(x => x.Before)
            .Must(x => x!.IsBase64())
            .WithMessage($"'{nameof(CursorPageDto.Before)}' must be a valid base64 string.")
            .When(x => x.Before != null);

        RuleFor(x => x.Order).NotEmpty()
            .WithMessage($"'{nameof(CursorPageDto.Order)}' must not be empty.")
            .ForEach(eachOrder =>
            {
                eachOrder.NotNull()
                    .WithMessage($"'{nameof(CursorPageDto.Order)}' contains a null element.")
                    .ChildRules(order =>
                    {
                        order.RuleFor(o => o.Field)
                            .NotEmpty()
                            .WithMessage("Order field must not be empty.");

                        order.RuleFor(o => o.Direction)
                            .NotNull()
                            .WithMessage("Order direction must be specified.")
                            .IsInEnum()
                            .WithMessage("Order direction must be a valid enum value.");
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
}