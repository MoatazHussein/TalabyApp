using FluentValidation;

namespace Talaby.Application.Features.Users.Commands.RegisterStore;

public class RegisterStoreCommandHandlerValidator : AbstractValidator<RegisterStoreCommand>
{
    public RegisterStoreCommandHandlerValidator()
    {
        RuleFor(dto => dto.FirstName).
            NotEmpty().WithMessage("Please provide a First Name")
            .Length(3, 50);

        //RuleFor(dto => dto.LastName).
        //    NotEmpty().WithMessage("Please provide a Last Name")
        //    .Length(3, 50);

        RuleFor(dto => dto.Email)
        .EmailAddress()
        .WithMessage("Please provide a valid email address");

        RuleFor(dto => dto.CommercialRegisterImageUrl)
            .NotEmpty().WithMessage("Please provide a Commercial Register Image Url");

        RuleFor(dto => dto.StoreCategoryId)
            .NotNull().NotEmpty().WithMessage("Please provide a store category");

    }
}
