using FluentValidation;
using Talaby.Domain.Enums;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequestStatus;

public class UpdateProjectRequestStatusCommandHandlerValidator : AbstractValidator<UpdateProjectRequestStatusCommand>
{
    public UpdateProjectRequestStatusCommandHandlerValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid status value.")
            .Must(s => s == ProjectRequestStatus.Cancelled)
            .WithMessage("Only Cancelled is allowed via this endpoint. Use dedicated endpoints for other transitions.");
    }
}
