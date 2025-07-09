using Talaby.Application.Common;

namespace Talaby.Application.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;

public interface IProjectQuestionReadRepository
{
    Task<PagedResult<ProjectQuestionListItemDto>> GetPagedQuestionsAsync(
        Guid projectRequestId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
