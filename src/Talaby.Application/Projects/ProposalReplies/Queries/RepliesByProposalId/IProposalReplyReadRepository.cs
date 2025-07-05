using Talaby.Application.Common;
using Talaby.Application.Projects.Dtos;

namespace Talaby.Application.Projects.ProposalReplies.Queries.RepliesByProposalId;

public interface IProposalReplyReadRepository
{
    Task<PagedResult<ProposalReplyDto>> GetPagedRepliesAsync(
        Guid proposalId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}




