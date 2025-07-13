using MediatR;
using Talaby.Application.Common;

namespace Talaby.Application.Features.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId
{
    public record GetQuestionsByProjectRequestIdQuery(
     Guid ProjectRequestId,
     int PageNumber,
     int PageSize
 ) : IRequest<PagedResult<ProjectQuestionListItemDto>>;

}
