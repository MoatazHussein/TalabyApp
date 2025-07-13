using MediatR;
namespace Talaby.Application.Features.Projects.ProposalReplies.Commands.UpdateProposalReply;

public class UpdateProposalReplyCommand : IRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = default!;
}