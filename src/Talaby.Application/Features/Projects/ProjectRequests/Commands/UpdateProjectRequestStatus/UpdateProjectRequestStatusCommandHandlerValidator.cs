using FluentValidation;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequestStatus;

public class UpdateProjectRequestStatusCommandHandlerValidator : AbstractValidator<UpdateProjectRequestStatusCommand>
{
    public UpdateProjectRequestStatusCommandHandlerValidator()
    {
        RuleFor(x => x.NewStatus)
      .IsInEnum()
      .WithMessage("Invalid status value. Must be one of: Open, Closed, Cancelled.");
    }
}
