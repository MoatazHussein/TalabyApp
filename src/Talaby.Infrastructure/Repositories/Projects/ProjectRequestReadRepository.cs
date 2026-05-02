using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests;
using Talaby.Application.Features.Projects.ProjectRequests.Queries.GetMyProjectRequests.Dtos;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities.Projects;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectRequestReadRepository(TalabyDbContext context) : IProjectRequestReadRepository
{
    public async Task<PagedResult<MyProjectRequestDto>> GetPagedClientProjectRequestsAsync(
        Guid clientId,
        int storeCategoryId,
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken)
    {
        var searchPhraseLower = searchPhrase?.ToLower();

        var baseQuery = context.ProjectRequests
            .AsNoTracking()
            .Where(r => r.CreatorId == clientId)
            .Where(r => searchPhraseLower == null
                        || r.Title.ToLower().Contains(searchPhraseLower)
                        || r.Description.ToLower().Contains(searchPhraseLower));

        if (storeCategoryId > 0)
        {
            baseQuery = baseQuery.Where(r => r.StoreCategoryId == storeCategoryId);
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var columnsSelector = new Dictionary<string, Expression<Func<ProjectRequest, object>>>
        {
            { nameof(ProjectRequest.CreatedAt), r => r.CreatedAt },
            { nameof(ProjectRequest.Title), r => r.Title },
            { nameof(ProjectRequest.Description), r => r.Description },
        };

        var sortColumn = sortBy ?? nameof(ProjectRequest.CreatedAt);
        var selectedColumn = columnsSelector.GetValueOrDefault(
            sortColumn,
            columnsSelector[nameof(ProjectRequest.CreatedAt)]);

        baseQuery = (sortDirection ?? SortDirection.Descending) == SortDirection.Ascending
            ? baseQuery.OrderBy(selectedColumn).ThenBy(r => r.Id)
            : baseQuery.OrderByDescending(selectedColumn).ThenByDescending(r => r.Id);

        var items = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .Select(r => new MyProjectRequestDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                ImageUrl = r.ImageUrl,
                MinBudget = r.MinBudget,
                MaxBudget = r.MaxBudget,
                StoreCategoryId = r.StoreCategoryId,
                CreatorId = r.CreatorId,
                CreatedAt = r.CreatedAt,
                Status = r.Status,
                CancellationReason = r.CancellationReason,
                CancelledAt = r.CancelledAtUtc,
                CancelledByUserId = r.CancelledByUserId,
                ProposalsCount = r.Proposals.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<MyProjectRequestDto>(items, totalCount, pageSize, pageNumber);
    }
}
