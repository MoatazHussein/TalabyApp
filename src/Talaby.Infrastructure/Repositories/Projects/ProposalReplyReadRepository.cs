using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.Dtos;
using Talaby.Application.Features.Projects.ProposalReplies.Queries.RepliesByProposalId;
using Talaby.Domain.Constants;
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
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken)
    {
        var proposal = await context.ProjectProposals
            .Include(q => q.Creator)
            .Include(q => q.ProjectRequest)
            .ThenInclude(r => r.Creator)
            .Where(q => q.Id == proposalId)
            .Select(q => new
            {
                Id = q.Id,
                ProposalCreatorId = q.Creator.Id,
                ProjectRequestId = q.ProjectRequest.Id,
                ProjectRequestCreatorId = q.ProjectRequest.Creator.Id,
                ProjectRequestCreatorEmail = q.ProjectRequest.Creator.Email,
                Content = q.Content,
                Status = q.Status,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (proposal == null)
        {
            throw new NotFoundException(nameof(ProjectProposal), proposalId.ToString());
        }

        var query = context.ProposalReplies
            .Where(r => r.ProjectProposalId == proposalId);

        var totalCount = await query.CountAsync(cancellationToken);

        var columnsSelector = new Dictionary<string, Expression<Func<ProposalReply, object>>>
        {
            { nameof(ProposalReply.CreatedAt), r => r.CreatedAt },
        };

        var sortColumn = sortBy ?? nameof(ProposalReply.CreatedAt);
        var selectedColumn = columnsSelector.GetValueOrDefault(
            sortColumn,
            columnsSelector[nameof(ProposalReply.CreatedAt)]);

        query = (sortDirection ?? SortDirection.Descending) == SortDirection.Ascending
            ? query.OrderBy(selectedColumn).ThenBy(r => r.Id)
            : query.OrderByDescending(selectedColumn).ThenByDescending(r => r.Id);

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
            ProposalCreatorId = proposal.ProposalCreatorId,
            ProjectRequestId = proposal.ProjectRequestId,
            ProjectRequestCreatorId = proposal.ProjectRequestCreatorId,
            ProjectRequestCreatorEmail = proposal.ProjectRequestCreatorEmail,
            ProposalContent = proposal.Content,
            ProposalStatus = proposal.Status,
            Replies = new PagedResult<ProposalReplyDto>(items, totalCount, pageSize, pageNumber)
        };
    }
}
