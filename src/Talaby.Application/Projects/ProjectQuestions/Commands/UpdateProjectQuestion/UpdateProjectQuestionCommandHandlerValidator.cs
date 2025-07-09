using FluentValidation;

namespace Talaby.Application.Projects.ProjectQuestions.Commands.UpdateProjectQuestion;

public class UpdateProjectQuestionCommandHandlerValidator : AbstractValidator<UpdateProjectQuestionCommand>
{
    public UpdateProjectQuestionCommandHandlerValidator()
    {
        RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters.");

    }
}
