using Talaby.Application.Common;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;

public interface IProjectProposalReadRepository
{
    Task<PagedResult<ProjectProposalListItemDto>> GetPagedProposalsAsync(
        Guid projectRequestId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken);
}
