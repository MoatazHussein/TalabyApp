using FluentValidation;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposalStatus;

public class UpdateProjectProposalStatusCommandHandlerValidator : AbstractValidator<UpdateProjectProposalStatusCommand>
{
    public UpdateProjectProposalStatusCommandHandlerValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid status value. Must be one of: Pending, Accepted, Rejected");

        RuleFor(x => x.CancellationReason)
            .MaximumLength(500)
            .When(x => x.CancellationReason is not null)
            .WithMessage("Cancellation reason must not exceed 500 characters.");
    }
}
