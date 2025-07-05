using MediatR;

namespace Talaby.Application.Projects.ProposalReplies.Commands.CreateProposalReply;
public record CreateProposalReplyCommand(
    Guid ProjectProposalId,
    string Content
) : IRequest<Guid>;

