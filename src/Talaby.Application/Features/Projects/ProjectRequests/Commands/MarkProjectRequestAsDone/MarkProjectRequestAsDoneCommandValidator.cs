using FluentValidation;

namespace Talaby.Application.Features.Projects.ProjectRequests.Commands.MarkProjectRequestAsDone;

public class MarkProjectRequestAsDoneCommandValidator : AbstractValidator<MarkProjectRequestAsDoneCommand>
{
    public MarkProjectRequestAsDoneCommandValidator()
    {
        RuleFor(x => x.ProjectRequestId)
            .NotEmpty()
            .WithMessage("ProjectRequestId is required.");
    }
}
