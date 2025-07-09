using Talaby.Application.Common;
using Talaby.Application.Projects.Dtos;

namespace Talaby.Application.Projects.QuestionReplies.Queries.RepliesByQuestionId;

public interface IQuestionReplyReadRepository
{
    Task<PagedResult<QuestionReplyDto>> GetPagedRepliesAsync(
        Guid questionId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}




