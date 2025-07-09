using MediatR;
using Talaby.Application.Common;
using Talaby.Application.Projects.Dtos;

namespace Talaby.Application.Projects.QuestionReplies.Queries.RepliesByQuestionId;

public record GetRepliesByQuestionIdQuery(
Guid QuestionId,
int PageNumber,
int PageSize
) : IRequest<PagedResult<QuestionReplyDto>>;
