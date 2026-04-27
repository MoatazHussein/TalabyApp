using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;
using Talaby.Domain.Constants;
using Talaby.Domain.Entities.Projects;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectQuestionReadRepository(TalabyDbContext context) : IProjectQuestionReadRepository
{
    public async Task<PagedResult<ProjectQuestionListItemDto>> GetPagedQuestionsAsync(
        Guid projectRequestId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken)
    {
        var baseQuery = context.ProjectQuestions
            .Where(p => p.ProjectRequestId == projectRequestId);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var columnsSelector = new Dictionary<string, Expression<Func<ProjectQuestion, object>>>
        {
            { nameof(ProjectQuestion.CreatedAt), p => p.CreatedAt },
        };

        var sortColumn = sortBy ?? nameof(ProjectQuestion.CreatedAt);
        var selectedColumn = columnsSelector.GetValueOrDefault(
            sortColumn,
            columnsSelector[nameof(ProjectQuestion.CreatedAt)]);

        baseQuery = (sortDirection ?? SortDirection.Descending) == SortDirection.Ascending
            ? baseQuery.OrderBy(selectedColumn).ThenBy(p => p.Id)
            : baseQuery.OrderByDescending(selectedColumn).ThenByDescending(p => p.Id);

        var items = await baseQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProjectQuestionListItemDto
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                CreatorEmail = p.Creator.Email,
                CreatorCommercialRegisterNumber = p.Creator.CommercialRegisterNumber,
                RepliesCount = p.Replies.Count
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProjectQuestionListItemDto>(items, totalCount, pageSize, pageNumber);
    }
}
