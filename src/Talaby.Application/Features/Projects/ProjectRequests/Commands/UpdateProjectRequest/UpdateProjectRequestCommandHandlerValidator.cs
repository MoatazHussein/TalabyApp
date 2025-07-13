using FluentValidation;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.UpdateProjectRequest;

public class UpdateProjectRequestCommandHandlerValidator : AbstractValidator<UpdateProjectRequestCommand>
{
    public UpdateProjectRequestCommandHandlerValidator()
    {
        RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.MinBudget)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum budget cannot be negative.")
                .LessThanOrEqualTo(x => x.MaxBudget).WithMessage("Minimum budget must be less than or equal to maximum budget.");

        RuleFor(x => x.MaxBudget)
                .GreaterThanOrEqualTo(0).WithMessage("Maximum budget cannot be negative.")
                .LessThanOrEqualTo(1000000).WithMessage("Maximum budget cannot exceed 1,000,000.");
    }
}
