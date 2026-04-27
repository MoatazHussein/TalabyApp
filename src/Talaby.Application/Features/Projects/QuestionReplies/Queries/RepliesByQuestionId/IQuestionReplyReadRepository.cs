using Talaby.Application.Features.Projects.Dtos;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;

public interface IQuestionReplyReadRepository
{
    Task<QuestionWithRepliesDto> GetQuestionWithRepliesAsync(
        Guid questionId,
        int pageNumber,
        int pageSize,
        string? sortBy,
        SortDirection? sortDirection,
        CancellationToken cancellationToken);
}
