using FluentValidation;

namespace Talaby.Application.Features.StoreCategories.Commands.CreateStoreCategory;

public class CreateStoreCategoryCommandHandlerValidator : AbstractValidator<CreateStoreCategoryCommand>
{
    public CreateStoreCategoryCommandHandlerValidator()
    {
        RuleFor(dto => dto.NameEn).
            NotEmpty().WithMessage("Please provide a valid English name")
            .Length(3, 50);

        RuleFor(dto => dto.NameAr).
            NotEmpty().WithMessage("Please provide a valid Arabic name")
            .Length(3, 50);
    }
}
