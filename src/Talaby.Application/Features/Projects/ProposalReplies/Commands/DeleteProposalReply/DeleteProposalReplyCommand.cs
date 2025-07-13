using MediatR;

namespace Talaby.Application.Features.Projects.ProposalReplies.Commands.DeleteProposalReply;

public class DeleteProposalReplyCommand(Guid id) : IRequest
{
    public Guid Id { get; } = id;
}
