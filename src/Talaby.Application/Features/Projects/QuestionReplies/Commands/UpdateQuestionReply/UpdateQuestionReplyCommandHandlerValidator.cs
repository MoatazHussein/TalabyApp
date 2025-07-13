using FluentValidation;

namespace Talaby.Application.Features.Projects.QuestionReplies.Commands.UpdateQuestionReply;

public class UpdateQuestionReplyCommandHandlerValidator : AbstractValidator<UpdateQuestionReplyCommand>
{
    public UpdateQuestionReplyCommandHandlerValidator()
    {
        RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters.");
    }
}
