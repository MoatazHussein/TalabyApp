using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.ProjectProposals.Queries.MyProjectProposals;
using Talaby.Application.Features.Projects.ProjectProposals.Queries.ProposalsByProjectRequestId;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities.Projects;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectProposalReadRepository(TalabyDbContext context) : IProjectProposalReadRepository
{
    public async Task<PagedResult<ProjectProposalListItemDto>> GetPagedProposalsAsync(
        Guid projectRequestId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken)
    {
        var baseQuery = context.ProjectProposals
            .Where(p => p.ProjectRequestId == projectRequestId);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var columnsSelector = new Dictionary<string, Expression<Func<ProjectProposal, object>>>
        {
            { nameof(ProjectProposal.CreatedAt), p => p.CreatedAt },
            { nameof(ProjectProposal.ProposedAmount), p => p.ProposedAmount },
        };

        var sortColumn = sortBy ?? nameof(ProjectProposal.CreatedAt);
        var selectedColumn = columnsSelector.GetValueOrDefault(
            sortColumn,
            columnsSelector[nameof(ProjectProposal.CreatedAt)]);

        baseQuery = (sortDirection ?? SortDirection.Descending) == SortDirection.Ascending
            ? baseQuery.OrderBy(selectedColumn).ThenBy(p => p.Id)
            : baseQuery.OrderByDescending(selectedColumn).ThenByDescending(p => p.Id);

        var items = await baseQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectProposalListItemDto
            {
                Id = p.Id,
                Content = p.Content,
                ProposedAmount = p.ProposedAmount,
                CreatedAt = p.CreatedAt,
                Status = p.Status,
                CancellationReason = p.CancellationReason,
                CancelledAt = p.CancelledAtUtc,
                CancelledByUserId = p.CancelledByUserId,
                CreatorEmail = p.Creator.Email,
                CreatorCommercialRegisterNumber = p.Creator.CommercialRegisterNumber!,
                RepliesCount = p.Replies.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProjectProposalListItemDto>(items, totalCount, pageSize, pageNumber);
    }

    public async Task<PagedResult<MyProjectProposalListItemDto>> GetPagedUserProposalsAsync(
        Guid creatorId,
        string? searchPhrase,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = context.ProjectProposals
            .AsNoTracking()
            .Where(p => p.CreatorId == creatorId)
            .Where(p => searchPhraseLower == null
                        || p.ProjectRequest!.Title.ToLower().Contains(searchPhraseLower)
                        || p.ProjectRequest.Description.ToLower().Contains(searchPhraseLower)
                        || p.Content.ToLower().Contains(searchPhraseLower));

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var columnsSelector = new Dictionary<string, Expression<Func<ProjectProposal, object>>>
        {
            { nameof(ProjectProposal.CreatedAt), p => p.CreatedAt },
            { nameof(ProjectProposal.ProposedAmount), p => p.ProposedAmount },
        };

        var sortColumn = sortBy ?? nameof(ProjectProposal.CreatedAt);
        var selectedColumn = columnsSelector.GetValueOrDefault(
            sortColumn,
            columnsSelector[nameof(ProjectProposal.CreatedAt)]);

        baseQuery = (sortDirection ?? SortDirection.Descending) == SortDirection.Ascending
            ? baseQuery.OrderBy(selectedColumn).ThenBy(p => p.Id)
            : baseQuery.OrderByDescending(selectedColumn).ThenByDescending(p => p.Id);

        var items = await baseQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new MyProjectProposalListItemDto
            {
                Id = p.Id,
                ProjectRequestId = p.ProjectRequestId,
                ProjectTitle = p.ProjectRequest!.Title,
                ProjectStatus = p.ProjectRequest.Status,
                StoreCategoryId = p.ProjectRequest.StoreCategoryId,
                ProjectMinBudget = p.ProjectRequest.MinBudget,
                ProjectMaxBudget = p.ProjectRequest.MaxBudget,
                ProjectCreatedAt = p.ProjectRequest.CreatedAt,
                Content = p.Content,
                ProposedAmount = p.ProposedAmount,
                CreatedAt = p.CreatedAt,
                Status = p.Status,
                CancellationReason = p.CancellationReason,
                CancelledAt = p.CancelledAtUtc,
                CancelledByUserId = p.CancelledByUserId,
                CreatorEmail = p.Creator!.Email!,
                CreatorCommercialRegisterNumber = p.Creator.CommercialRegisterNumber!,
                RepliesCount = p.Replies.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<MyProjectProposalListItemDto>(items, totalCount, pageSize, pageNumber);
    }
}
