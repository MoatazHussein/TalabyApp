using FluentValidation;

namespace Talaby.Application.Features.Projects.ProjectProposals.Commands.UpdateProjectProposalStatus;

public class UpdateProjectProposalStatusCommandHandlerValidator : AbstractValidator<UpdateProjectProposalStatusCommand>
{
    public UpdateProjectProposalStatusCommandHandlerValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid status value. Must be one of: Pending, Accepted, Rejected");
    }
}
