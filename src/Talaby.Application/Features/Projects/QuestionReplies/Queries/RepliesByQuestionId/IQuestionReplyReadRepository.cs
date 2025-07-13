using Talaby.Application.Features.Projects.Dtos;

namespace Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;

public interface IQuestionReplyReadRepository
{
    Task<QuestionWithRepliesDto> GetQuestionWithRepliesAsync(Guid questionId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}




