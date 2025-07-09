using Microsoft.EntityFrameworkCore;
using Talaby.Application.Common;
using Talaby.Application.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;
using Talaby.Infrastructure.Persistence;

namespace Talaby.Infrastructure.Repositories.Projects;

public class ProjectQuestionReadRepository(TalabyDbContext context) : IProjectQuestionReadRepository
{
    public async Task<PagedResult<ProjectQuestionListItemDto>> GetPagedQuestionsAsync(
        Guid projectRequestId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = context.ProjectQuestions
            .Where(p => p.ProjectRequestId == projectRequestId)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await baseQuery.CountAsync(cancellationToken);

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
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProjectQuestionListItemDto>(items, totalCount, pageSize, pageNumber);
    }
}
