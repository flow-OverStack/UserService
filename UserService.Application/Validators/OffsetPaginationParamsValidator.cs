using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Application.Resources;
using UserService.Application.Settings;
using UserService.Domain.Dtos.Pagination;

namespace UserService.Application.Validators;

public class OffsetPaginationParamsValidator : AbstractValidator<OffsetPaginationParams>
{
    public OffsetPaginationParamsValidator(IOptions<PaginationRules> pagination)
    {
        var maxPageSize = pagination.Value.MaxPageSize;

        RuleFor(x => x.Skip)
            .NotNull().WithMessage(_ => string.Format(ErrorMessage.Required, nameof(OffsetPaginationParams.Skip)))
            .GreaterThanOrEqualTo(0)
            .WithMessage(_ => string.Format(ErrorMessage.InvalidMinValue, nameof(OffsetPaginationParams.Skip), 0));

        RuleFor(x => x.Take)
            .NotNull().WithMessage(_ => string.Format(ErrorMessage.Required, nameof(OffsetPaginationParams.Take)))
            .InclusiveBetween(0, maxPageSize)
            .WithMessage(_ =>
                string.Format(ErrorMessage.InvalidRange, nameof(OffsetPaginationParams.Take), maxPageSize));
    }
}