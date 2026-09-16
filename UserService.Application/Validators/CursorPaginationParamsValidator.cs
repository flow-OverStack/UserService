using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Application.Resources;
using UserService.Application.Settings;
using UserService.Domain.Dtos.Pagination;
using UserService.Domain.Extensions;

namespace UserService.Application.Validators;

public class CursorPaginationParamsValidator : AbstractValidator<CursorPaginationParams>
{
    public CursorPaginationParamsValidator(IOptions<PaginationRules> pagination)
    {
        RuleFor(x => x.First)
            .InclusiveBetween(0, pagination.Value.MaxPageSize)
            .WithMessage(_ => string.Format(ErrorMessage.InvalidRange, nameof(CursorPaginationParams.First),
                pagination.Value.MaxPageSize))
            .When(x => x.First.HasValue);

        RuleFor(x => x.Last)
            .InclusiveBetween(0, pagination.Value.MaxPageSize)
            .WithMessage(_ => string.Format(ErrorMessage.InvalidRange, nameof(CursorPaginationParams.Last),
                pagination.Value.MaxPageSize))
            .When(x => x.Last.HasValue);

        RuleFor(x => x.After)
            .Must(x => x!.IsBase64())
            .WithMessage(_ => string.Format(ErrorMessage.InvalidCursorFormat, nameof(CursorPaginationParams.After)))
            .When(x => x.After != null);

        RuleFor(x => x.Before)
            .Must(x => x!.IsBase64())
            .WithMessage(_ => string.Format(ErrorMessage.InvalidCursorFormat, nameof(CursorPaginationParams.Before)))
            .When(x => x.Before != null);

        RuleFor(x => x.Order).NotEmpty()
            .WithMessage(_ => string.Format(ErrorMessage.RequiredNonEmpty, nameof(CursorPaginationParams.Order)))
            .ForEach(eachOrder =>
            {
                eachOrder.NotNull()
                    .WithMessage(_ => string.Format(ErrorMessage.CollectionContainsNullElement,
                        nameof(CursorPaginationParams.Order)))
                    .ChildRules(order =>
                    {
                        order.RuleFor(o => o.Field)
                            .NotEmpty()
                            .WithMessage(ErrorMessage.OrderFieldRequired);

                        order.RuleFor(o => o.Direction)
                            .NotNull()
                            .WithMessage(ErrorMessage.OrderDirectionRequired)
                            .IsInEnum()
                            .WithMessage(ErrorMessage.InvalidOrderDirection);
                    });
            });

        var errorMessage = string.Format(
            ErrorMessage.ConflictingPaginationArguments,
            nameof(CursorPaginationParams.First).LowercaseFirstLetter(),
            nameof(CursorPaginationParams.After).LowercaseFirstLetter(),
            nameof(CursorPaginationParams.Last).LowercaseFirstLetter(),
            nameof(CursorPaginationParams.Before).LowercaseFirstLetter());
        RuleFor(x => x)
            .Must(x =>
                x is { First: not null, Last: null, Before: null }
                    or { Last: not null, First: null, After: null })
            .WithMessage(errorMessage);
    }
}