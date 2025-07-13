using Talaby.Application.Features.Projects.Dtos;

namespace Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;

public interface IProposalReplyReadRepository
{
    Task<ProposalWithRepliesDto> GetProposalWithRepliesAsync(
        Guid proposalId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}




