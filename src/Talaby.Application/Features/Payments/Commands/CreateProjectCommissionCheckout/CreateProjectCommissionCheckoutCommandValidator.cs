using FluentValidation;

namespace Talaby.Application.Features.Payments.Commands.CreateProjectCommissionCheckout;

public class CreateProjectCommissionCheckoutCommandValidator
    : AbstractValidator<CreateProjectCommissionCheckoutCommand>
{
    public CreateProjectCommissionCheckoutCommandValidator()
    {
        RuleFor(x => x.ProjectRequestId)
            .NotEmpty()
            .WithMessage("ProjectRequestId is required.");
    }
}
