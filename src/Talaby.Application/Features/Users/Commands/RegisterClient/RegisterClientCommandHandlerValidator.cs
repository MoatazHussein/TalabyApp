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

        RuleFor(dto => dto.Password)
            .NotEmpty().WithMessage("Please provide a Password")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Passwords must have at least one uppercase ('A'-'Z').")
            .Matches(@"[0-9]").WithMessage("Passwords must have at least one digit ('0'-'9').")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Passwords must have at least one non alphanumeric character.");
    }
}