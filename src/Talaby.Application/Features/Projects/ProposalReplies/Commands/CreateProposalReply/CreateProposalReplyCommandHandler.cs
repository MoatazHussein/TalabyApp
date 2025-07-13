using MediatR;
using Talaby.Application.Features.Users;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Repositories.Projects;

namespace Talaby.Application.Features.Projects.ProposalReplies.Commands.CreateProposalReply;

public class CreateProposalReplyCommandHandler(
    IProposalReplyRepository repository,
    IUserContext currentUser) : IRequestHandler<CreateProposalReplyCommand, Guid>
{
    public async Task<Guid> Handle(CreateProposalReplyCommand request, CancellationToken cancellationToken)
    {
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
