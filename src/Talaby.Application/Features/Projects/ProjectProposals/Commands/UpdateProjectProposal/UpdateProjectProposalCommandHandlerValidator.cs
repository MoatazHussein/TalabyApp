using FluentValidation;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposal;

public class UpdateProjectProposalCommandHandlerValidator : AbstractValidator<UpdateProjectProposalCommand>
{
    public UpdateProjectProposalCommandHandlerValidator()
    {
        RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters.");

        RuleFor(x => x.ProposedAmount)
                .GreaterThanOrEqualTo(0).WithMessage("Proposed Amount cannot be negative.")
                .LessThanOrEqualTo(1000000).WithMessage("Proposed Amount cannot exceed 1,000,000.");
    }
}
