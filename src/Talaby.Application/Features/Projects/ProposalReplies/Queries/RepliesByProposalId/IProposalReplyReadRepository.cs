using Talaby.Application.Features.Projects.Dtos;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;

public interface IProposalReplyReadRepository
{
    Task<ProposalWithRepliesDto> GetProposalWithRepliesAsync(
        Guid proposalId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken);
}
