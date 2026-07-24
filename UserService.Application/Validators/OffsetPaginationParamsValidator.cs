using FluentValidation;
using Microsoft.Extensions.Options;
using UserService.Application.Settings;
using UserService.Domain.Dtos.Pagination;

namespace UserService.Application.Validators;

public class OffsetPaginationParamsValidator : AbstractValidator<OffsetPaginationParams>
{
    public OffsetPaginationParamsValidator(IOptions<PaginationRules> pagination)
    {
        var maxPageSize = pagination.Value.MaxPageSize;

        RuleFor(x => x.Skip)
            .NotNull().WithMessage($"'{nameof(OffsetPaginationParams.Skip)}' must be provided.")
            .GreaterThanOrEqualTo(0).WithMessage($"'{nameof(OffsetPaginationParams.Skip)}' must be greater than or equal to 0.");

        RuleFor(x => x.Take)
            .NotNull().WithMessage($"'{nameof(OffsetPaginationParams.Take)}' must be provided.")
            .InclusiveBetween(0, maxPageSize)
            .WithMessage($"'{nameof(OffsetPaginationParams.Take)}' must be between 0 and {maxPageSize}.");
    }
}