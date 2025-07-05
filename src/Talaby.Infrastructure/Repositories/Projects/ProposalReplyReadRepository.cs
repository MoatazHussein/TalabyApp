using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Projects.Dtos;
using Talaby.Application.Projects.ProposalReplies.Queries.RepliesByProposalId;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProposalReplyReadRepository(TalabyDbContext context) : IProposalReplyReadRepository
{
    public async Task<PagedResult<ProposalReplyDto>> GetPagedRepliesAsync(
        Guid proposalId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.ProposalReplies
            .Where(r => r.ProjectProposalId == proposalId)
            .OrderBy(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ProposalReplyDto
            {
                Id = r.Id,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                CreatorEmail = r.Creator.Email
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProposalReplyDto>(items, totalCount, pageSize, pageNumber);
    }
}
