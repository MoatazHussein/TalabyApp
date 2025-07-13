using MediatR;
using Talaby.Application.Features.Projects.Dtos;

namespace Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;

public record GetRepliesByQuestionIdQuery(
Guid QuestionId,
int PageNumber,
int PageSize
) : IRequest<QuestionWithRepliesDto>;
