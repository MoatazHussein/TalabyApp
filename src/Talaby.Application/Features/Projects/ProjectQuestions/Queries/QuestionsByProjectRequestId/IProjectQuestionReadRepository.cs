using Talaby.Application.Common;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;

public interface IProjectQuestionReadRepository
{
    Task<PagedResult<ProjectQuestionListItemDto>> GetPagedQuestionsAsync(
        Guid projectRequestId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken);
}
