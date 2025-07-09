using MediatR;
using Talaby.Application.Common;

namespace Talaby.Application.Projects.ProjectQuestions.Queries.QuestionsByProjectRequestId;

public class GetQuestionsByProjectRequestIdHandler(IProjectQuestionReadRepository repository)
        : IRequestHandler<GetQuestionsByProjectRequestIdQuery, PagedResult<ProjectQuestionListItemDto>>
{
    public async Task<PagedResult<ProjectQuestionListItemDto>> Handle(
        GetQuestionsByProjectRequestIdQuery request,
        CancellationToken cancellationToken)
    {
        return await repository.GetPagedQuestionsAsync(
            request.ProjectRequestId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
