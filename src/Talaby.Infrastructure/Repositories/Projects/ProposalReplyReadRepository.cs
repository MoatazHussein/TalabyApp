using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.Dtos;
using Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;
using Talaby.Domain.Entities.Projects;
using Talaby.Domain.Exceptions;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProposalReplyReadRepository(TalabyDbContext context) : IProposalReplyReadRepository
{
    public async Task<ProposalWithRepliesDto> GetProposalWithRepliesAsync(
  Guid proposalId,
  int pageNumber,
  int pageSize,
  CancellationToken cancellationToken)
    {
        var proposal = await context.ProjectProposals
            .Where(q => q.Id == proposalId)
            .Select(q => new
            {
                q.Id,
                q.Content,
            })
            .FirstOrDefaultAsync(cancellationToken);

         if (proposal == null)
         throw new NotFoundException(nameof(ProjectProposal), proposalId.ToString());

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
                CreatorEmail = r.Creator.Email,
                CreatorCommercialRegisterNumber = r.Creator.CommercialRegisterNumber
            })
            .ToListAsync(cancellationToken);

        return new ProposalWithRepliesDto
        {
            ProposalId = proposal.Id,
            ProposalContent = proposal.Content,
            Replies = new PagedResult<ProposalReplyDto>(items, totalCount, pageSize, pageNumber)
        };

    }
}
