using FluentValidation;

namespace Talaby.Application.Features.Projects.ProposalReplies.Commands.UpdateProposalReply;

public class UpdateProposalReplyCommandHandlerValidator : AbstractValidator<UpdateProposalReplyCommand>
{
    public UpdateProposalReplyCommandHandlerValidator()
    {
        RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters.");
    }
}
