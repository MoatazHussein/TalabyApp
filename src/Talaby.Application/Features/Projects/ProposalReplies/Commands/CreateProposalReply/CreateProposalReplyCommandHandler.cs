using MediatR;
using Talaby.Application.Features.Users.Services;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProposalReplies.Commands.CreateProposalReply;

public class CreateProposalReplyCommandHandler(
    IProposalReplyRepository repository,
    IUserContext currentUser,
    IUserConfirmationGuard userConfirmationGuard) : IRequestHandler<CreateProposalReplyCommand, Guid>
{
    public async Task<Guid> Handle(CreateProposalReplyCommand request, CancellationToken cancellationToken)
    {
        await userConfirmationGuard.EnsureCurrentUserEmailConfirmedAsync(cancellationToken);

        var reply = new ProposalReply
        {
            Id = Guid.NewGuid(),
            ProjectProposalId = request.ProjectProposalId,
            Content = request.Content,
            CreatorId = currentUser.GetCurrentUser().Id,
            CreatedAt = DateTime.UtcNow
        };

        await repository.Create(reply);
        return reply.Id;
    }
}
