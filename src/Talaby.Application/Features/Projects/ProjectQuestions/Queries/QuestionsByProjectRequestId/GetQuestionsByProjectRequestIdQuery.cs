using MediatR;
using Talaby.Application.Common;
using Talaby.Domain.Constants;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;

public record GetQuestionsByProjectRequestIdQuery(
    Guid ProjectRequestId,
    int PageNumber,
    int PageSize,
    string? SortBy = null,
    SortDirection? SortDirection = null
) : IRequest<PagedResult<ProjectQuestionListItemDto>>;
