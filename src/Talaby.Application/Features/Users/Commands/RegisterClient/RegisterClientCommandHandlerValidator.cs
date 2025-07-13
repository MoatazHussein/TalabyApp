using FluentValidation;

namespace Talaby.Application.Features.Users.Commands.RegisterClient;

public class RegisterClientCommandHandlerValidator : AbstractValidator<RegisterClientCommand>
{
    public RegisterClientCommandHandlerValidator()
    {
        RuleFor(dto => dto.FirstName).
            NotEmpty().WithMessage("Please provide a First Name")
            .Length(3, 50);

        RuleFor(dto => dto.LastName).
            NotEmpty().WithMessage("Please provide a Last Name")
            .Length(3, 50);

        RuleFor(dto => dto.Email)
        .EmailAddress()
        .WithMessage("Please provide a valid email address");


    }
}