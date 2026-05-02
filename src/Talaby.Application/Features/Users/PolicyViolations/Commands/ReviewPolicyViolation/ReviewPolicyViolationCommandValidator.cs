using FluentValidation;
using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Users.PolicyViolations.Commands.ReviewPolicyViolation;

public sealed class ReviewPolicyViolationCommandValidator
    : AbstractValidator<ReviewPolicyViolationCommand>
{
    public ReviewPolicyViolationCommandValidator()
    {
        RuleFor(command => command.ViolationId)
            .NotEmpty()
            .WithMessage("Violation id is required");

        RuleFor(command => command.ReviewStatus)
            .IsInEnum()
            .Must(status => status is UserPolicyViolationReviewStatus.Confirmed
                or UserPolicyViolationReviewStatus.Waived)
            .WithMessage("ReviewStatus must be Confirmed or Waived");

        RuleFor(command => command.ReviewNote)
            .MaximumLength(500);
    }
}
