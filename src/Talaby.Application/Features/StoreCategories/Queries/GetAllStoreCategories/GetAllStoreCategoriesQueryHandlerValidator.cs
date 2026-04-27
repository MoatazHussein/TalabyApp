using FluentValidation;
using Talaby.Domain.Entities;

namespace Talaby.Application.Features.StoreCategories.Queries.GetAllStoreCategories;

public class GetAllStoreCategoriesQueryHandlerValidator : AbstractValidator<GetAllStoreCategoriesQuery>
{
    private readonly string[] allowedSortByColumnNames =
    [
        nameof(StoreCategory.NameAr),
        nameof(StoreCategory.NameEn)
    ];


    public GetAllStoreCategoriesQueryHandlerValidator()
    {
        RuleFor(r => r.PageNumber)
            .GreaterThan(0);

        RuleFor(r => r.PageSize)
            .GreaterThan(0);

        RuleFor(r => r.SortBy)
            .Must(value => allowedSortByColumnNames.Contains(value))
            .When(q => q.SortBy != null)
            .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}
