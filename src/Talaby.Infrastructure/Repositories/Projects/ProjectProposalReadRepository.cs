using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectProposalReadRepository(TalabyDbContext context) : IProjectProposalReadRepository
{
    public async Task<PagedResult<ProjectProposalListItemDto>> GetPagedProposalsAsync(
        Guid projectRequestId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = context.ProjectProposals
            .Where(p => p.ProjectRequestId == projectRequestId)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var items = await baseQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectProposalListItemDto
            {
                Id = p.Id,
                Content = p.Content,
                ProposedAmount = p.ProposedAmount,
                CreatedAt = p.CreatedAt,
                CreatorEmail = p.Creator.Email,
                CreatorCommercialRegisterNumber = p.Creator.CommercialRegisterNumber,
                RepliesCount = p.Replies.Count
            })
            .OrderBy(p => p.ProposedAmount).ThenBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProjectProposalListItemDto>(items, totalCount, pageSize, pageNumber);
    }
}
