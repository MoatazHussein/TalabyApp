using Talaby.Application.Common;

namespace Talaby.Application.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;

public interface IProjectProposalReadRepository
{
    Task<PagedResult<ProjectProposalListItemDto>> GetPagedProposalsAsync(
        Guid projectRequestId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
