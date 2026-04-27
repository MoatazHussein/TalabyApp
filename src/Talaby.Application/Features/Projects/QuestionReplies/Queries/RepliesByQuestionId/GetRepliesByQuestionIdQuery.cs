using MediatR;
using Talaby.Application.Features.Projects.Dtos;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.QuestionReplies.Queries.RepliesByQuestionId;

public record GetRepliesByQuestionIdQuery(
    Guid QuestionId,
    int PageNumber,
    int PageSize,
    string? SortBy = null,
    SortDirection? SortDirection = null
) : IRequest<QuestionWithRepliesDto>;
